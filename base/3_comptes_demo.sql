-- =====================================================================
--  PharmaStock — Comptes de demonstration pour l'application VB.NET
--  A executer APRES 1_schema.sql (et, si souhaite, 2_triggers_et_donnees_test.sql).
--
--  L'application hache les mots de passe en SHA-256 (voir SecurityHelper.vb).
--  Ce script (re)definit deux comptes avec des mots de passe connus :
--      login "admin"   / mot de passe "admin123"     (role administrateur)
--      login "employe" / mot de passe "employe123"   (role employe)
--
--  IMPORTANT : ce sont des comptes de DEMONSTRATION. A ne pas utiliser en
--  production ; prevoir un hachage avec sel (ex. BCrypt) pour la version finale.
-- =====================================================================

USE pharmastock;

-- Administrateur
INSERT INTO utilisateur (nom, login, mot_de_passe_hash, id_role, actif)
VALUES ('Pharmacien Titulaire', 'admin',
        '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
        (SELECT id_role FROM role WHERE libelle = 'administrateur'), 1)
ON DUPLICATE KEY UPDATE
    mot_de_passe_hash = VALUES(mot_de_passe_hash),
    actif = 1;

-- Employe
INSERT INTO utilisateur (nom, login, mot_de_passe_hash, id_role, actif)
VALUES ('Vendeur Officine', 'employe',
        'c422c301b9f734203ab9cbb82979ca5fed6dc23d858f74fcb6512d1003147ec1',
        (SELECT id_role FROM role WHERE libelle = 'employe'), 1)
ON DUPLICATE KEY UPDATE
    mot_de_passe_hash = VALUES(mot_de_passe_hash),
    actif = 1;
