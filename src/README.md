# PharmaStock — Application VB.NET (WinForms)

Application de bureau Windows (monoposte) de gestion de stock d'officine, développée en **VB.NET** et connectée à la base **MySQL** `pharmastock`. Ce dossier contient le code source de départ.

## Prérequis

- **Windows** + **Visual Studio 2022** (charge de travail « Développement .NET Desktop »).
- **.NET Framework 4.8** (Developer Pack).
- **MySQL 8.0+** avec la base `pharmastock` créée (voir les scripts SQL à la racine du dépôt).

## Mise en route

1. Créer la base et charger les données :
   ```bash
   mysql -u root -p < ../pharmastock.sql
   mysql -u root -p pharmastock < ../pharmastock_triggers_et_donnees_test.sql
   mysql -u root -p pharmastock < comptes_demo.sql
   ```
2. Ouvrir `PharmaStock/PharmaStock.sln` dans Visual Studio.
3. Vérifier la chaîne de connexion dans `PharmaStock/App.config` (`Server`, `Uid`, `Pwd`).
4. Restaurer les paquets NuGet (MySql.Data se télécharge automatiquement) puis lancer (F5).

## Comptes de démonstration

| Login | Mot de passe | Rôle |
| --- | --- | --- |
| `admin` | `admin123` | administrateur |
| `employe` | `employe123` | employé |

> Les mots de passe sont hachés en SHA-256 côté application. Pour la version finale, prévoir un hachage avec sel (ex. BCrypt).

## Architecture (couches)

```
Program.vb                 Point d'entrée (Sub Main)
Data/Database.vb           Accès MySQL (connexion + helpers)
Helpers/SecurityHelper.vb  Hachage des mots de passe
Models/                    Entités (Produit, Utilisateur)
Repositories/              Accès aux données par entité (CRUD)
Forms/                     Interfaces : connexion, menu, produits, édition
```

## Écrans déjà fonctionnels

- **Connexion** (`FrmLogin`) — authentification sur la table `utilisateur`.
- **Menu principal** (`FrmPrincipal`) — conteneur MDI, affiche l'utilisateur connecté.
- **Gestion des produits** (`FrmProduits` + `FrmProduitEdition`) — liste, recherche, ajout, modification, désactivation.

Les modules Stock, Ventes/Facturation et Livraisons sont prévus (menus grisés) et seront ajoutés sur le même modèle (Model → Repository → Form).

## Remarque

Ce code n'a pas été compilé dans l'environnement de préparation (pas de Windows/.NET Desktop) ; il doit être ouvert et exécuté dans Visual Studio. Signaler toute erreur de compilation pour correction.
