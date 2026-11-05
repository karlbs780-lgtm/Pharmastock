Imports System.Data

' Lecture de l'ordonnancier des substances reglementees.
Public Class OrdonnancierRepository

    Public Function Lister() As DataTable
        Return Database.ExecuterTable(
            "SELECT o.id_ordonnancier, o.date_delivrance, p.nom_commercial, l.numero_lot, " &
            "o.quantite, o.nom_prescripteur, o.nom_patient " &
            "FROM ordonnancier o " &
            "JOIN produit p ON p.id_produit = o.id_produit " &
            "JOIN lot l ON l.id_lot = o.id_lot " &
            "ORDER BY o.date_delivrance DESC")
    End Function

End Class
