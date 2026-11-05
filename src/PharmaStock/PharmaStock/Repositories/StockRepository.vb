Imports System.Data

' Lecture du stock et des alertes (s'appuie sur les vues SQL).
Public Class StockRepository

    Public Function StockParProduit() As DataTable
        Return Database.ExecuterTable(
            "SELECT nom_commercial, stock_disponible, seuil_min FROM vue_stock_produit ORDER BY nom_commercial")
    End Function

    Public Function Lots() As DataTable
        Return Database.ExecuterTable(
            "SELECT p.nom_commercial, l.numero_lot, l.date_peremption, l.quantite, l.statut " &
            "FROM lot l JOIN produit p ON p.id_produit = l.id_produit " &
            "ORDER BY p.nom_commercial, l.date_peremption")
    End Function

    Public Function AlertesRupture() As DataTable
        Return Database.ExecuterTable(
            "SELECT nom_commercial, stock_disponible, seuil_min FROM vue_alerte_rupture ORDER BY nom_commercial")
    End Function

    Public Function AlertesPeremption() As DataTable
        Return Database.ExecuterTable(
            "SELECT nom_commercial, numero_lot, date_peremption, quantite, jours_restants " &
            "FROM vue_alerte_peremption ORDER BY date_peremption")
    End Function

End Class
