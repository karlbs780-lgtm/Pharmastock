# PharmaStock — Base de données

Base de données **MySQL 8.0** du logiciel **PharmaStock**, un logiciel de gestion de stock pour officine de pharmacie (application de bureau Windows développée en VB.NET, monoposte). Référentiel métier : **DPML — Cameroun**.

## Contenu du dépôt

| Fichier | Description |
| --- | --- |
| `pharmastock.sql` | Schéma complet : 18 tables, contraintes, clés étrangères, 3 vues et données de référence. |
| `pharmastock_triggers_et_donnees_test.sql` | Déclencheurs (triggers) + jeu de données de test. À exécuter **après** `pharmastock.sql`. |

## Installation

```bash
mysql -u root -p < pharmastock.sql
mysql -u root -p pharmastock < pharmastock_triggers_et_donnees_test.sql
```

> `pharmastock.sql` commence par `DROP DATABASE IF EXISTS pharmastock;` : le relancer réinitialise entièrement la base.

Pour un schéma visuel : MySQL Workbench → *Database → Reverse Engineer* sur la base `pharmastock`.

## Ce que contient la base (18 tables, 6 domaines)

- **Produits & lots** — catalogue (nom commercial, DCI, forme, dosage, prix, seuils, n° AMM, homologation) et lots avec numéro, date de péremption et quantité.
- **Approvisionnement** — fournisseurs agréés (dont la CENAME), commandes et lignes de commande.
- **Livraisons fournisseurs** — bons de livraison rapprochés aux commandes, quantités reçues, écarts et litiges.
- **Ventes & facturation client** — clients, factures et lignes de facture (produit + lot délivré).
- **Stock** — journal de tous les mouvements (entrées, sorties, retours, ajustements, destructions).
- **Réglementaire & sécurité** — ordonnancier des substances réglementées, utilisateurs/rôles, journal d'audit.

## Ce que la base fait automatiquement

- **Stock en temps réel** : stock d'un produit = somme des lots actifs (vue `vue_stock_produit`).
- **Mise à jour automatique du stock** : à chaque mouvement, `lot.quantite` est recalculé (trigger) et le lot passe en `epuise` à zéro.
- **FEFO** : lots indexés par date de péremption (le plus proche sort en premier).
- **Alertes** : `vue_alerte_rupture` (stock ≤ seuil minimal) et `vue_alerte_peremption` (lots périmant sous 90 jours).
- **Ordonnancier** : chaque délivrance de substance réglementée est enregistrée (prescripteur, patient).

## Ce que la base garantit

- Stock jamais négatif ; sortie refusée si stock insuffisant.
- **Ordonnancier et journal des mouvements inaltérables** (ni modification ni suppression) — traçabilité, conservation 10 ans.
- Suppression impossible d'un produit / lot / utilisateur référencé par un historique.
- Réceptions toujours rattachées à un fournisseur agréé.
- Mots de passe chiffrés, accès par rôle (administrateur / employé).

## Hors périmètre (versions ultérieures)

Gestion de la paie, comptabilité générale, tiers payant, vente en ligne.

---

*Projet de groupe — module Introduction au génie logiciel.*
