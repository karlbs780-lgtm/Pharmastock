# PharmaStock

**Logiciel de gestion de stock pour officine de pharmacie** — application de bureau Windows (monoposte) en **VB.NET**, avec une base de données **MySQL 8.0**. Référentiel métier : **DPML — Cameroun**.

Projet de groupe — module *Introduction au génie logiciel*.

## Contenu du dépôt

```
PharmaStock.sln            ← à ouvrir dans Visual Studio
Guide_installation_PharmaStock.pdf
base/                      ← base de données MySQL
   1_schema.sql                  tables, contraintes, vues, données de référence
   2_triggers_et_donnees_test.sql  triggers + jeu de données de test
   3_comptes_demo.sql            comptes admin / employé
   mise_a_jour_01.sql            mise à jour d'une base déjà installée
   mettre_a_jour_base.bat        applique les mises à jour (double-clic)
app/                       ← code de l'application (VB.NET WinForms)
```

Le guide PDF détaille toute l'installation, avec des captures d'écran.

## Installation

### 1. Base de données

Exécuter les trois scripts **dans l'ordre** :

```bash
mysql -u root -p < base/1_schema.sql
mysql -u root -p pharmastock < base/2_triggers_et_donnees_test.sql
mysql -u root -p pharmastock < base/3_comptes_demo.sql
```

> `1_schema.sql` commence par `DROP DATABASE IF EXISTS pharmastock;` : le relancer **efface toute la base**. Les triggers du script 2 sont indispensables (stock, ventes, livraisons).

**Base déjà installée ?** Après un `git pull`, ne réinstallez pas tout : double-cliquez sur `base/mettre_a_jour_base.bat`. Il trouve `mysql.exe` (PATH, XAMPP, MySQL Server ou WAMP), demande le mot de passe une fois (vide avec XAMPP) et applique les scripts `mise_a_jour_*.sql`, sans perte de données.

### 2. Application

1. Ouvrir `PharmaStock.sln` dans Visual Studio 2022 (ou `dotnet build PharmaStock.sln -c Release`).
2. Vérifier la chaîne de connexion dans `app/App.config` (`Server`, `Port`, `Uid`, `Pwd`).
3. Lancer avec **F5**. Les paquets NuGet (MySql.Data) sont restaurés automatiquement.

**Comptes de démonstration** : `admin / admin123` (administrateur) · `employe / employe123` (employé).

> **Chemins trop longs** : si `git clone` échoue avec *Filename too long*, ou si l'exécutable affiche « Erreur irrécupérable » au démarrage, cloner dans un dossier court (ex. `C:\dev\Pharmastock`) ou activer `git config --global core.longpaths true`.

## Fonctionnalités

- **Connexion** et menu principal (MDI) avec **accès par rôle** : l'employé gère les clients, les ventes et consulte le stock ; produits, fournisseurs, livraisons et ordonnancier sont réservés à l'administrateur.
- **Produits**, **Fournisseurs**, **Clients** — gestion complète (CRUD) ; un produit peut être classé dans un tableau de substances réglementées (I à IV).
- **Livraison fournisseur** — crée ou alimente les lots ; refus des lots déjà périmés et des fournisseurs non agréés.
- **Vente / Facturation** — lots choisis en **FEFO** (répartition sur plusieurs lots si nécessaire), total avec remise, **ordonnancier automatique** pour les substances réglementées.
- **État du stock** et **Tableau de bord** — stock par produit, lots, alertes de rupture et de péremption (< 90 jours).
- **Ordonnancier** — consultation en lecture seule.

### Règles garanties par la base (triggers)

- Stock en temps réel ; stock jamais négatif ; lot marqué `epuise` à zéro.
- Un lot périmé ne peut plus être vendu (seule la destruction reste possible).
- Livraison refusée si le fournisseur n'est pas agréé.
- Ordonnancier et journal des mouvements inaltérables (conservation 10 ans).

## Organisation du code (`app/`)

Tous les fichiers sont au même niveau ; le nom indique le rôle :

| Fichiers | Rôle |
| --- | --- |
| `Program.vb` | Point d'entrée (connexion puis fenêtre principale). |
| `Frm*.vb` | Écrans : `FrmLogin`, `FrmPrincipal`, `FrmProduits`, `FrmVente`, `FrmLivraison`, `FrmStock`, `FrmTableauBord`, `FrmOrdonnancier`… |
| `*Repository.vb` | Accès aux données par entité (requêtes SQL, transactions). |
| `Produit.vb`, `Client.vb`, `LigneVente.vb`… | Entités manipulées par les écrans. |
| `Database.vb`, `DbUtil.vb` | Connexion MySQL et utilitaires communs. |
| `SecurityHelper.vb` | Hachage des mots de passe (SHA-256). |

## Hors périmètre

Gestion de la paie, comptabilité générale, tiers payant, vente en ligne.
