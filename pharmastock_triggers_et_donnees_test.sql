-- =====================================================================
--  PharmaStock — Declencheurs (triggers) + jeu de donnees de test
--  A executer APRES pharmastock.sql, sur la base fraichement creee.
--    mysql -u root -p pharmastock < pharmastock_triggers_et_donnees_test.sql
--  (les INSERT de test supposent une base vide : les identifiants
--   auto-increment commencent a 1.)
-- =====================================================================

USE pharmastock;

-- ---------------------------------------------------------------------
--  PARTIE 1 — DECLENCHEURS
-- ---------------------------------------------------------------------
DELIMITER $$

-- 1a. Controle AVANT un mouvement : stock suffisant pour une sortie (RG-03)
DROP TRIGGER IF EXISTS trg_mouvement_before_insert$$
CREATE TRIGGER trg_mouvement_before_insert
BEFORE INSERT ON mouvement_stock
FOR EACH ROW
BEGIN
    DECLARE v_stock INT;
    IF NEW.type_mouvement IN ('sortie','destruction') THEN
        SELECT quantite INTO v_stock FROM lot WHERE id_lot = NEW.id_lot;
        IF v_stock < NEW.quantite THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Stock insuffisant pour ce mouvement de sortie.';
        END IF;
    END IF;
END$$

-- 1b. Mise a jour du stock du lot APRES le mouvement (+ bascule du statut)
DROP TRIGGER IF EXISTS trg_mouvement_after_insert$$
CREATE TRIGGER trg_mouvement_after_insert
AFTER INSERT ON mouvement_stock
FOR EACH ROW
BEGIN
    IF NEW.type_mouvement IN ('entree','retour') THEN
        UPDATE lot SET quantite = quantite + NEW.quantite WHERE id_lot = NEW.id_lot;
    ELSEIF NEW.type_mouvement IN ('sortie','destruction') THEN
        UPDATE lot SET quantite = quantite - NEW.quantite WHERE id_lot = NEW.id_lot;
    ELSE  -- ajustement : valeur signee (peut etre negative)
        UPDATE lot SET quantite = quantite + NEW.quantite WHERE id_lot = NEW.id_lot;
    END IF;

    UPDATE lot
       SET statut = CASE
                       WHEN quantite <= 0 AND statut <> 'perime' THEN 'epuise'
                       WHEN quantite  > 0 AND statut = 'epuise'  THEN 'actif'
                       ELSE statut
                    END
     WHERE id_lot = NEW.id_lot;
END$$

-- 1c. Ordonnancier immuable (RG-09 / RG-10) : ni modification, ni suppression
DROP TRIGGER IF EXISTS trg_ordonnancier_no_update$$
CREATE TRIGGER trg_ordonnancier_no_update
BEFORE UPDATE ON ordonnancier
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ordonnancier : modification interdite (ecriture definitive).';
END$$

DROP TRIGGER IF EXISTS trg_ordonnancier_no_delete$$
CREATE TRIGGER trg_ordonnancier_no_delete
BEFORE DELETE ON ordonnancier
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ordonnancier : suppression interdite (conservation 10 ans).';
END$$

-- 1d. Journal des mouvements immuable (RG-09)
DROP TRIGGER IF EXISTS trg_mouvement_no_update$$
CREATE TRIGGER trg_mouvement_no_update
BEFORE UPDATE ON mouvement_stock
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Mouvement de stock : modification interdite.';
END$$

DROP TRIGGER IF EXISTS trg_mouvement_no_delete$$
CREATE TRIGGER trg_mouvement_no_delete
BEFORE DELETE ON mouvement_stock
FOR EACH ROW
BEGIN
    SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Mouvement de stock : suppression interdite.';
END$$

DELIMITER ;

-- ---------------------------------------------------------------------
--  PARTIE 2 — JEU DE DONNEES DE TEST
--  (les hash de mot de passe ci-dessous sont fictifs, pour la demo)
-- ---------------------------------------------------------------------

-- Utilisateurs (role 1 = administrateur, role 2 = employe)
INSERT INTO utilisateur (nom, login, mot_de_passe_hash, id_role) VALUES
 ('Pharmacien Titulaire', 'admin',   '$2y$10$hashFictifAdmin0000000000000000000000000000000', 1),
 ('Vendeur Officine',     'employe', '$2y$10$hashFictifEmploye000000000000000000000000000000', 2);

-- Categories
INSERT INTO categorie (libelle) VALUES
 ('Antalgiques'), ('Antibiotiques'), ('Antipaludiques'), ('Psychotropes');

-- Fournisseurs agrees
INSERT INTO fournisseur (nom, agree, telephone, actif) VALUES
 ('CENAME',           1, '+237 222 00 00 00', 1),
 ('Laborex Cameroun', 1, '+237 233 00 00 00', 1);

-- Clients (le 1er sert de "client de passage")
INSERT INTO client (nom, telephone) VALUES
 ('Client de passage', NULL),
 ('Jean Mballa',       '+237 699 00 00 00');

-- Produits (id_tableau = 4 -> Tableau IV pour le Valium)
INSERT INTO produit
 (code_barres, nom_commercial, dci, forme, dosage, id_categorie, numero_amm,
  statut_homologation, sur_ordonnance, id_tableau, prix_achat, prix_vente, seuil_min) VALUES
 ('6001','Doliprane 500','Paracetamol','Comprime','500 mg',1,'AMM-CM-001','homologue',0,NULL, 300, 500,20),
 ('6002','Clamoxyl 500','Amoxicilline','Gelule','500 mg',2,'AMM-CM-002','homologue',1,NULL,1500,2500,15),
 ('6003','Coartem','Artemether + Lumefantrine','Comprime','20/120 mg',3,'AMM-CM-003','homologue',1,NULL,1200,2000,10),
 ('6004','Valium 5','Diazepam','Comprime','5 mg',4,'AMM-CM-004','homologue',1,4, 800,1500, 5);

-- Lots (quantite initiale 0 : alimentee ensuite par les mouvements d'entree)
INSERT INTO lot (id_produit, numero_lot, date_peremption, quantite, id_fournisseur, date_reception, statut) VALUES
 (1,'LOT-A1','2027-06-30',0,1,CURDATE(),'actif'),                                 -- Doliprane, lot long
 (2,'LOT-B1','2026-12-31',0,2,CURDATE(),'actif'),                                 -- Clamoxyl
 (3,'LOT-C1','2027-03-31',0,1,CURDATE(),'actif'),                                 -- Coartem
 (4,'LOT-D1','2027-09-30',0,2,CURDATE(),'actif'),                                 -- Valium
 (1,'LOT-A2', DATE_ADD(CURDATE(), INTERVAL 45 DAY),0,1,CURDATE(),'actif');        -- Doliprane, peremption proche (alerte)

-- Entrees en stock : les triggers mettent a jour lot.quantite
INSERT INTO mouvement_stock (id_lot, type_mouvement, quantite, id_utilisateur, motif) VALUES
 (1,'entree',100,1,'Stock initial'),
 (2,'entree', 40,1,'Stock initial'),
 (3,'entree', 60,1,'Stock initial'),
 (4,'entree', 12,1,'Stock initial'),
 (5,'entree',  8,1,'Stock initial (lot court)');

-- Exemple d'approvisionnement + livraison rapprochee
INSERT INTO commande_fournisseur (id_fournisseur, statut, id_utilisateur, montant_total)
 VALUES (1,'recue',1,30000);
INSERT INTO ligne_commande (id_commande, id_produit, quantite_commandee, quantite_recue, prix_unitaire)
 VALUES (1,1,100,100,300);
INSERT INTO livraison (id_fournisseur, id_commande, numero_bon_livraison, date_livraison, statut, id_utilisateur)
 VALUES (1,1,'BL-2026-001',CURDATE(),'complete',1);
INSERT INTO ligne_livraison (id_livraison, id_produit, id_lot, numero_lot, date_peremption, quantite_livree, quantite_conforme)
 VALUES (1,1,1,'LOT-A1','2027-06-30',100,100);

-- Exemple de vente (facturation) :
--   ligne 1 : 2 Doliprane sortis du lot le plus proche de la peremption (FEFO -> LOT-A2)
--   ligne 2 : 1 Valium (substance reglementee) -> enregistrement a l'ordonnancier
INSERT INTO vente (numero_facture, id_client, id_utilisateur, remise, montant_total)
 VALUES ('FAC-2026-0001', 2, 2, 0, 2500);

INSERT INTO ligne_vente (id_vente, id_produit, id_lot, quantite, prix_unitaire, montant)
 VALUES (1,1,5,2,500,1000);
INSERT INTO mouvement_stock (id_lot, type_mouvement, quantite, id_utilisateur, motif, reference)
 VALUES (5,'sortie',2,2,'Vente','VENTE:1');

INSERT INTO ligne_vente (id_vente, id_produit, id_lot, quantite, prix_unitaire, montant)
 VALUES (1,4,4,1,1500,1500);
INSERT INTO mouvement_stock (id_lot, type_mouvement, quantite, id_utilisateur, motif, reference)
 VALUES (4,'sortie',1,2,'Vente','VENTE:1');
INSERT INTO ordonnancier (id_produit, id_lot, id_ligne_vente, quantite, nom_prescripteur, nom_patient, id_utilisateur)
 VALUES (4,4,2,1,'Dr. Nkolo','Jean Mballa',2);

-- ---------------------------------------------------------------------
--  PARTIE 3 — VERIFICATIONS RAPIDES (demonstration)
-- ---------------------------------------------------------------------
-- Stock disponible par produit (Doliprane = 100 + 6 apres vente = 106 ; Valium = 11)
SELECT * FROM vue_stock_produit;

-- Lots perimant sous 90 jours (LOT-A2 doit apparaitre)
SELECT * FROM vue_alerte_peremption;

-- Produits sous le seuil minimal (aucun au depart)
SELECT * FROM vue_alerte_rupture;

-- Contenu de l'ordonnancier (la delivrance de Valium doit y figurer)
SELECT id_ordonnancier, id_produit, quantite, nom_prescripteur, nom_patient, date_delivrance
FROM ordonnancier;

-- =====================================================================
--  Fin du script
-- =====================================================================
