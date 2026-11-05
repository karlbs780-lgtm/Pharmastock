-- =====================================================================
--  PharmaStock — Migration 001 : correctifs peremption + fournisseur agree
--  (PR #1 « Correction des bugs trouves lors des tests »)
--
--  A executer UNE FOIS sur une base pharmastock DEJA INSTALLEE.
--  Ce script ne supprime AUCUNE donnee : il remplace seulement deux
--  declencheurs (triggers). Il peut etre relance sans risque.
--
--      mysql -u root -p pharmastock < migrations/001_correctifs_peremption_fournisseur.sql
--
--  (ou double-cliquer sur mettre_a_jour_base.bat a la racine du depot)
--
--  Une base creee a partir de zero avec pharmastock.sql puis
--  pharmastock_triggers_et_donnees_test.sql contient deja ces triggers.
-- =====================================================================

USE pharmastock;

DELIMITER $$

-- Controle AVANT un mouvement :
--   - stock suffisant pour une sortie ou une destruction (RG-03)
--   - un lot perime ne peut plus etre vendu (seule la destruction reste possible)
DROP TRIGGER IF EXISTS trg_mouvement_before_insert$$
CREATE TRIGGER trg_mouvement_before_insert
BEFORE INSERT ON mouvement_stock
FOR EACH ROW
BEGIN
    DECLARE v_stock INT;
    DECLARE v_peremption DATE;
    IF NEW.type_mouvement IN ('sortie','destruction') THEN
        SELECT quantite, date_peremption INTO v_stock, v_peremption FROM lot WHERE id_lot = NEW.id_lot;
        IF v_stock < NEW.quantite THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Stock insuffisant pour ce mouvement de sortie.';
        END IF;
        IF NEW.type_mouvement = 'sortie' AND v_peremption <= CURDATE() THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Lot perime : vente interdite.';
        END IF;
    END IF;
END$$

-- Reception uniquement aupres d'un fournisseur agree (RG-12)
DROP TRIGGER IF EXISTS trg_livraison_before_insert$$
CREATE TRIGGER trg_livraison_before_insert
BEFORE INSERT ON livraison
FOR EACH ROW
BEGIN
    IF NOT EXISTS (SELECT 1 FROM fournisseur
                   WHERE id_fournisseur = NEW.id_fournisseur AND agree = 1) THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Livraison refusee : fournisseur non agree.';
    END IF;
END$$

DELIMITER ;

-- Verification : les deux triggers doivent apparaitre ci-dessous.
SELECT trigger_name AS trigger_installe
FROM information_schema.triggers
WHERE trigger_schema = 'pharmastock'
  AND trigger_name IN ('trg_mouvement_before_insert', 'trg_livraison_before_insert');
