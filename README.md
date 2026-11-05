# PharmaStock

**Logiciel de gestion de stock pour officine de pharmacie** — application de bureau Windows (monoposte) développée en **VB.NET**, avec une base de données **MySQL 8.0**. Référentiel métier : **DPML — Cameroun**.

Projet de groupe — module *Introduction au génie logiciel*.

## Structure du dépôt

| Élément | Description |
| --- | --- |
| `pharmastock.sql` | Base de données : 18 tables, contraintes, clés étrangères, 3 vues et données de référence. |
| `pharmastock_triggers_et_donnees_test.sql` | Déclencheurs (triggers) + jeu de données de test. À exécuter **après** `pharmastock.sql`. |
| `src/` | Application VB.NET (WinForms) — solution Visual Studio, voir [`src/README.md`](src/README.md). |
| `src/comptes_demo.sql` | Comptes de démonstration pour se connecter à l'application. |

## Installation

### 1. Base de données (MySQL 8.0+)

```bash
mysql -u root -p < pharmastock.sql
mysql -u root -p pharmastock < pharmastock_triggers_et_donnees_test.sql
mysql -u root -p pharmastock < src/comptes_demo.sql
```

> `pharmastock.sql` commence par `DROP DATABASE IF EXISTS pharmastock;` : le relancer réinitialise entièrement la base. Les triggers sont **indispensables** au bon fonctionnement du stock, des ventes et des livraisons.

### 2. Application (Windows + Visual Studio 2022)

1. Ouvrir `src/PharmaStock/PharmaStock.sln`.
2. Vérifier la chaîne de connexion dans `src/PharmaStock/PharmaStock/App.config` (`Server`, `Uid`, `Pwd`).
3. Restaurer les paquets NuGet (MySql.Data) puis lancer (F5).

**Comptes de démonstration** : `admin / admin123` (administrateur) · `employe / employe123` (employé).

## Fonctionnalités

### Application (écrans)

- **Connexion** et menu principal (MDI) par utilisateur et rôle.
- **Produits**, **Fournisseurs**, **Clients** — gestion complète (CRUD).
- **Livraison fournisseur** — saisie d'un bon ; crée/alimente les lots et génère les entrées en stock.
- **Vente / Facturation** — choix du lot en **FEFO**, calcul du total, **passage automatique à l'ordonnancier** pour les substances réglementées.
- **État du stock** et **Tableau de bord** — stock par produit, lots, alertes de rupture et de péremption.
- **Ordonnancier** — consultation en lecture seule.

### Base de données (6 domaines)

- **Produits & lots** — catalogue (nom, DCI, forme, dosage, prix, seuils, n° AMM, homologation) et lots (numéro, péremption, quantité).
- **Approvisionnement** — fournisseurs agréés (dont la CENAME), commandes et lignes.
- **Livraisons fournisseurs** — bons de livraison, quantités reçues, écarts et litiges.
- **Ventes & facturation client** — clients, factures et lignes (produit + lot délivré).
- **Stock** — journal de tous les mouvements (entrées, sorties, retours, ajustements, destructions).
- **Réglementaire & sécurité** — ordonnancier, utilisateurs/rôles, journal d'audit.

### Règles appliquées automatiquement (triggers & vues)

- **Stock en temps réel** et mise à jour automatique de `lot.quantite` à chaque mouvement ; lot en `epuise` à zéro.
- **FEFO** : le lot à péremption la plus proche sort en premier.
- **Alertes** : rupture (stock ≤ seuil minimal) et péremption (< 90 jours).
- Stock jamais négatif ; **sortie refusée** si stock insuffisant.
- **Ordonnancier et journal inaltérables** (ni modification ni suppression) — conservation 10 ans.
- Réceptions rattachées à un fournisseur agréé ; mots de passe chiffrés ; accès par rôle.

## Architecture de l'application (couches)

```
Program.vb                 Point d'entrée (Sub Main)
Data/Database.vb           Accès MySQL (connexion + helpers)
Helpers/                   Hachage des mots de passe, utilitaires BDD
Models/                    Entités (Produit, Fournisseur, Client, LigneVente, …)
Repositories/              Accès aux données par entité (CRUD, transactions)
Forms/                     Interfaces (connexion, menu, produits, ventes, livraisons, …)
```

## État d'avancement

Fonctionnels : base de données complète (schéma, triggers, données de test) et application couvrant produits, fournisseurs, clients, livraisons, ventes (FEFO + ordonnancier), stock et alertes.

**Hors périmètre (versions ultérieures)** : gestion de la paie, comptabilité générale, tiers payant, vente en ligne.

> L'application est à compiler et exécuter sous Windows (Visual Studio) ; signaler toute erreur de compilation pour correction.
