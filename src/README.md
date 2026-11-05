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

## Écrans fonctionnels

- **Connexion** (`FrmLogin`) — authentification sur la table `utilisateur`.
- **Menu principal** (`FrmPrincipal`) — conteneur MDI, affiche l'utilisateur connecté.
- **Produits** (`FrmProduits` + `FrmProduitEdition`) — liste, recherche, CRUD.
- **Fournisseurs** (`FrmFournisseurs` + `FrmFournisseurEdition`) — CRUD.
- **Clients** (`FrmClients` + `FrmClientEdition`) — liste, ajout, modification.
- **Livraison fournisseur** (`FrmLivraison`) — saisie d'un bon de livraison ; crée/alimente les lots et génère les entrées en stock.
- **Vente / Facturation** (`FrmVente`) — choix du lot en **FEFO**, calcul du total, et **passage automatique à l'ordonnancier** pour les substances réglementées.
- **État du stock** (`FrmStock`) — stock par produit, lots, alertes rupture et péremption.
- **Tableau de bord** (`FrmTableauBord`) — ruptures et péremptions à l'écran.
- **Ordonnancier** (`FrmOrdonnancier`) — consultation en lecture seule.

## Important — déclencheurs requis

Les écrans **Livraison** et **Vente** insèrent des mouvements dans `mouvement_stock` et s'appuient sur les **triggers** (`pharmastock_triggers_et_donnees_test.sql`) pour mettre à jour `lot.quantite` et contrôler le stock. Installer ce script avant de tester ces écrans.

## Remarque

Ce code n'a pas été compilé dans l'environnement de préparation (pas de Windows/.NET Desktop) ; il doit être ouvert et exécuté dans Visual Studio. Signaler toute erreur de compilation pour correction.
