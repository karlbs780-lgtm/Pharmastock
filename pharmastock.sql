-- =====================================================================
--  PharmaStock — Base de donnees de gestion de stock d'officine
--  SGBD : MySQL 8.0+  (moteur InnoDB, encodage utf8mb4)
--  Portee : stock, lots/peremption (FEFO), approvisionnement,
--           livraisons fournisseurs, facturation client, ordonnancier,
--           utilisateurs/securite.  (La gestion de la paie est hors perimetre.)
--  Montants exprimes en FCFA (DECIMAL).
-- =====================================================================

DROP DATABASE IF EXISTS pharmastock;
CREATE DATABASE pharmastock CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE pharmastock;

-- ---------------------------------------------------------------------
-- 1. Tables de reference (independantes)
-- ---------------------------------------------------------------------

CREATE TABLE role (
    id_role      INT UNSIGNED NOT NULL AUTO_INCREMENT,
    libelle      VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id_role),
    UNIQUE KEY uq_role_libelle (libelle)
) ENGINE=InnoDB;

CREATE TABLE categorie (
    id_categorie INT UNSIGNED NOT NULL AUTO_INCREMENT,
    libelle      VARCHAR(100) NOT NULL,
    PRIMARY KEY (id_categorie),
    UNIQUE KEY uq_categorie_libelle (libelle)
) ENGINE=InnoDB;

-- Tableaux des substances reglementees (Loi n 97/019 : tableaux I a IV)
CREATE TABLE tableau_substance (
    id_tableau   INT UNSIGNED NOT NULL AUTO_INCREMENT,
    code         VARCHAR(5)   NOT NULL,
    libelle      VARCHAR(100) NOT NULL,
    PRIMARY KEY (id_tableau),
    UNIQUE KEY uq_tableau_code (code)
) ENGINE=InnoDB;

-- Parametres configurables (ex : seuil de pre-peremption)
CREATE TABLE parametre (
    id_parametre INT UNSIGNED NOT NULL AUTO_INCREMENT,
    cle          VARCHAR(50)  NOT NULL,
    valeur       VARCHAR(255) NOT NULL,
    description  VARCHAR(255) NULL,
    PRIMARY KEY (id_parametre),
    UNIQUE KEY uq_parametre_cle (cle)
) ENGINE=InnoDB;

CREATE TABLE fournisseur (
    id_fournisseur INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom         VARCHAR(150) NOT NULL,
    agree       TINYINT(1)   NOT NULL DEFAULT 1,   -- fournisseur agree (RG-12)
    telephone   VARCHAR(30)  NULL,
    email       VARCHAR(120) NULL,
    adresse     VARCHAR(255) NULL,
    actif       TINYINT(1)   NOT NULL DEFAULT 1,
    PRIMARY KEY (id_fournisseur)
) ENGINE=InnoDB;

CREATE TABLE client (
    id_client   INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom         VARCHAR(150) NOT NULL,
    telephone   VARCHAR(30)  NULL,
    adresse     VARCHAR(255) NULL,
    date_creation DATETIME   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_client)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 2. Utilisateurs et securite
-- ---------------------------------------------------------------------

CREATE TABLE utilisateur (
    id_utilisateur     INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom                VARCHAR(100) NOT NULL,
    login              VARCHAR(50)  NOT NULL,
    mot_de_passe_hash  VARCHAR(255) NOT NULL,       -- jamais en clair (ENF-03)
    id_role            INT UNSIGNED NOT NULL,
    actif              TINYINT(1)   NOT NULL DEFAULT 1,
    date_creation      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_utilisateur),
    UNIQUE KEY uq_utilisateur_login (login),
    CONSTRAINT fk_utilisateur_role FOREIGN KEY (id_role)
        REFERENCES role (id_role) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Journal d'audit des operations sensibles (EF-26, ENF-04)
CREATE TABLE journal_activite (
    id_journal     INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_utilisateur INT UNSIGNED NOT NULL,
    action         VARCHAR(100) NOT NULL,
    entite         VARCHAR(80)  NULL,
    id_entite      INT UNSIGNED NULL,
    details        VARCHAR(500) NULL,
    date_heure     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_journal),
    CONSTRAINT fk_journal_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 3. Produits et lots
-- ---------------------------------------------------------------------

CREATE TABLE produit (
    id_produit     INT UNSIGNED NOT NULL AUTO_INCREMENT,
    code_barres    VARCHAR(50)  NULL,
    nom_commercial VARCHAR(150) NOT NULL,
    dci            VARCHAR(150) NULL,              -- Denomination Commune Internationale
    forme          VARCHAR(80)  NULL,
    dosage         VARCHAR(80)  NULL,
    id_categorie   INT UNSIGNED NULL,
    numero_amm     VARCHAR(80)  NULL,              -- Autorisation de Mise sur le Marche
    statut_homologation ENUM('homologue','non_homologue','retire') NOT NULL DEFAULT 'homologue',
    sur_ordonnance TINYINT(1)   NOT NULL DEFAULT 0,
    id_tableau     INT UNSIGNED NULL,              -- substance reglementee (NULL si libre)
    prix_achat     DECIMAL(12,2) NOT NULL DEFAULT 0,
    prix_vente     DECIMAL(12,2) NOT NULL DEFAULT 0,
    seuil_min      INT UNSIGNED NOT NULL DEFAULT 0,
    seuil_max      INT UNSIGNED NULL,
    actif          TINYINT(1)   NOT NULL DEFAULT 1,
    PRIMARY KEY (id_produit),
    UNIQUE KEY uq_produit_code_barres (code_barres),
    KEY idx_produit_nom (nom_commercial),
    KEY idx_produit_dci (dci),
    CONSTRAINT fk_produit_categorie FOREIGN KEY (id_categorie)
        REFERENCES categorie (id_categorie) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_produit_tableau FOREIGN KEY (id_tableau)
        REFERENCES tableau_substance (id_tableau) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Un lot = une entree identifiee par numero + peremption (EF-11)
CREATE TABLE lot (
    id_lot         INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_produit     INT UNSIGNED NOT NULL,
    numero_lot     VARCHAR(80)  NOT NULL,
    date_peremption DATE        NOT NULL,
    quantite       INT          NOT NULL DEFAULT 0,  -- stock du lot
    id_fournisseur INT UNSIGNED NULL,                -- origine du lot
    date_reception DATE         NULL,
    statut ENUM('actif','quarantaine','perime','epuise') NOT NULL DEFAULT 'actif',
    PRIMARY KEY (id_lot),
    UNIQUE KEY uq_lot_produit_numero (id_produit, numero_lot),
    KEY idx_lot_peremption (date_peremption),        -- utilise par le FEFO (EF-12)
    CONSTRAINT fk_lot_produit FOREIGN KEY (id_produit)
        REFERENCES produit (id_produit) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_lot_fournisseur FOREIGN KEY (id_fournisseur)
        REFERENCES fournisseur (id_fournisseur) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT chk_lot_quantite CHECK (quantite >= 0)  -- RG-03 (MySQL 8.0.16+)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 4. Approvisionnement (commandes fournisseurs)
-- ---------------------------------------------------------------------

CREATE TABLE commande_fournisseur (
    id_commande    INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_fournisseur INT UNSIGNED NOT NULL,
    date_commande  DATE         NOT NULL DEFAULT (CURRENT_DATE),
    statut ENUM('en_cours','partiellement_recue','recue','annulee') NOT NULL DEFAULT 'en_cours',
    id_utilisateur INT UNSIGNED NOT NULL,
    montant_total  DECIMAL(12,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_commande),
    CONSTRAINT fk_commande_fournisseur FOREIGN KEY (id_fournisseur)
        REFERENCES fournisseur (id_fournisseur) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_commande_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ligne_commande (
    id_ligne_commande INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_commande       INT UNSIGNED NOT NULL,
    id_produit        INT UNSIGNED NOT NULL,
    quantite_commandee INT UNSIGNED NOT NULL,
    quantite_recue     INT UNSIGNED NOT NULL DEFAULT 0,
    prix_unitaire      DECIMAL(12,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_ligne_commande),
    CONSTRAINT fk_lc_commande FOREIGN KEY (id_commande)
        REFERENCES commande_fournisseur (id_commande) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_lc_produit FOREIGN KEY (id_produit)
        REFERENCES produit (id_produit) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 5. Livraisons fournisseurs (EF-36 a EF-39)
-- ---------------------------------------------------------------------

CREATE TABLE livraison (
    id_livraison        INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_fournisseur      INT UNSIGNED NOT NULL,
    id_commande         INT UNSIGNED NULL,          -- rapprochement commande/livraison (EF-37)
    numero_bon_livraison VARCHAR(80) NULL,
    date_livraison      DATE         NOT NULL,
    statut ENUM('complete','partielle','litige') NOT NULL DEFAULT 'complete',
    id_utilisateur      INT UNSIGNED NOT NULL,
    PRIMARY KEY (id_livraison),
    CONSTRAINT fk_livraison_fournisseur FOREIGN KEY (id_fournisseur)
        REFERENCES fournisseur (id_fournisseur) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_livraison_commande FOREIGN KEY (id_commande)
        REFERENCES commande_fournisseur (id_commande) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_livraison_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ligne_livraison (
    id_ligne_livraison INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_livraison       INT UNSIGNED NOT NULL,
    id_produit         INT UNSIGNED NOT NULL,
    id_lot             INT UNSIGNED NULL,           -- lot cree/alimente a la reception
    numero_lot         VARCHAR(80)  NOT NULL,
    date_peremption    DATE         NOT NULL,
    quantite_livree    INT UNSIGNED NOT NULL,
    quantite_conforme  INT UNSIGNED NOT NULL,       -- ecart = livree - conforme (EF-37)
    motif_ecart        VARCHAR(255) NULL,           -- litige / non conformite (EF-39)
    PRIMARY KEY (id_ligne_livraison),
    CONSTRAINT fk_ll_livraison FOREIGN KEY (id_livraison)
        REFERENCES livraison (id_livraison) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_ll_produit FOREIGN KEY (id_produit)
        REFERENCES produit (id_produit) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_ll_lot FOREIGN KEY (id_lot)
        REFERENCES lot (id_lot) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 6. Ventes et facturation client (EF-31 a EF-35)
-- ---------------------------------------------------------------------

CREATE TABLE vente (
    id_vente       INT UNSIGNED NOT NULL AUTO_INCREMENT,
    numero_facture VARCHAR(50)  NULL,
    id_client      INT UNSIGNED NULL,               -- NULL = client de passage
    date_vente     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_utilisateur INT UNSIGNED NOT NULL,
    remise         DECIMAL(12,2) NOT NULL DEFAULT 0,
    montant_total  DECIMAL(12,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (id_vente),
    UNIQUE KEY uq_vente_facture (numero_facture),
    CONSTRAINT fk_vente_client FOREIGN KEY (id_client)
        REFERENCES client (id_client) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_vente_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ligne_vente (
    id_ligne_vente INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_vente       INT UNSIGNED NOT NULL,
    id_produit     INT UNSIGNED NOT NULL,
    id_lot         INT UNSIGNED NOT NULL,           -- lot delivre (FEFO, EF-34)
    quantite       INT UNSIGNED NOT NULL,
    prix_unitaire  DECIMAL(12,2) NOT NULL,
    remise_ligne   DECIMAL(12,2) NOT NULL DEFAULT 0,
    montant        DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (id_ligne_vente),
    CONSTRAINT fk_lv_vente FOREIGN KEY (id_vente)
        REFERENCES vente (id_vente) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_lv_produit FOREIGN KEY (id_produit)
        REFERENCES produit (id_produit) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_lv_lot FOREIGN KEY (id_lot)
        REFERENCES lot (id_lot) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 7. Mouvements de stock (journal, EF-09 / EF-30)
-- ---------------------------------------------------------------------

CREATE TABLE mouvement_stock (
    id_mouvement   INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_lot         INT UNSIGNED NOT NULL,
    type_mouvement ENUM('entree','sortie','retour','ajustement','destruction') NOT NULL,
    quantite       INT          NOT NULL,           -- quantite du mouvement (positive)
    date_mouvement DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_utilisateur INT UNSIGNED NOT NULL,
    motif          VARCHAR(255) NULL,
    reference      VARCHAR(80)  NULL,               -- ex : 'VENTE:123', 'LIVRAISON:45'
    PRIMARY KEY (id_mouvement),
    KEY idx_mouvement_date (date_mouvement),
    CONSTRAINT fk_mouvement_lot FOREIGN KEY (id_lot)
        REFERENCES lot (id_lot) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_mouvement_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 8. Ordonnancier des substances reglementees (EF-21/22, RG-08 a RG-10)
--    Enregistrement immuable, a conserver 10 ans.
-- ---------------------------------------------------------------------

CREATE TABLE ordonnancier (
    id_ordonnancier INT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_produit      INT UNSIGNED NOT NULL,
    id_lot          INT UNSIGNED NOT NULL,
    id_ligne_vente  INT UNSIGNED NULL,
    quantite        INT UNSIGNED NOT NULL,
    nom_prescripteur VARCHAR(150) NOT NULL,
    nom_patient     VARCHAR(150) NOT NULL,
    date_delivrance DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_utilisateur  INT UNSIGNED NOT NULL,
    PRIMARY KEY (id_ordonnancier),
    KEY idx_ordonnancier_date (date_delivrance),
    CONSTRAINT fk_ord_produit FOREIGN KEY (id_produit)
        REFERENCES produit (id_produit) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_ord_lot FOREIGN KEY (id_lot)
        REFERENCES lot (id_lot) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_ord_ligne_vente FOREIGN KEY (id_ligne_vente)
        REFERENCES ligne_vente (id_ligne_vente) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_ord_utilisateur FOREIGN KEY (id_utilisateur)
        REFERENCES utilisateur (id_utilisateur) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 9. Vues utiles (alertes et stock)
-- ---------------------------------------------------------------------

-- Stock disponible par produit (somme des lots actifs)
CREATE OR REPLACE VIEW vue_stock_produit AS
SELECT  p.id_produit,
        p.nom_commercial,
        p.seuil_min,
        COALESCE(SUM(CASE WHEN l.statut = 'actif' THEN l.quantite ELSE 0 END), 0) AS stock_disponible
FROM    produit p
LEFT JOIN lot l ON l.id_produit = p.id_produit
GROUP BY p.id_produit, p.nom_commercial, p.seuil_min;

-- Alerte de rupture : stock <= seuil minimal (EF-17 / RG-07)
CREATE OR REPLACE VIEW vue_alerte_rupture AS
SELECT  id_produit, nom_commercial, stock_disponible, seuil_min
FROM    vue_stock_produit
WHERE   stock_disponible <= seuil_min;

-- Alerte de peremption : lots actifs perimant sous 90 jours (EF-13 / RG-06)
CREATE OR REPLACE VIEW vue_alerte_peremption AS
SELECT  l.id_lot, l.id_produit, p.nom_commercial, l.numero_lot,
        l.date_peremption, l.quantite,
        DATEDIFF(l.date_peremption, CURDATE()) AS jours_restants
FROM    lot l
JOIN    produit p ON p.id_produit = l.id_produit
WHERE   l.statut = 'actif'
  AND   l.date_peremption <= DATE_ADD(CURDATE(), INTERVAL 90 DAY);

-- ---------------------------------------------------------------------
-- 10. Donnees de reference initiales
-- ---------------------------------------------------------------------

INSERT INTO role (libelle) VALUES
    ('administrateur'),
    ('employe');

INSERT INTO tableau_substance (code, libelle) VALUES
    ('I',   'Tableau I'),
    ('II',  'Tableau II'),
    ('III', 'Tableau III'),
    ('IV',  'Tableau IV');

INSERT INTO parametre (cle, valeur, description) VALUES
    ('seuil_preperemption_jours', '90', 'Nombre de jours avant peremption declenchant une alerte'),
    ('devise', 'FCFA', 'Devise utilisee pour les montants'),
    ('duree_conservation_ordonnancier_ans', '10', 'Duree legale de conservation de l''ordonnancier');

-- =====================================================================
--  Fin du script
-- =====================================================================
