' Ligne d'une vente en cours de saisie.
Public Class LigneVente
    Public Property IdProduit As Integer
    Public Property Produit As String
    Public Property IdLot As Integer
    Public Property NumeroLot As String
    Public Property Quantite As Integer
    Public Property PrixUnitaire As Decimal
    Public Property Reglemente As Boolean
    Public Property Prescripteur As String
    Public Property Patient As String

    Public ReadOnly Property Montant As Decimal
        Get
            Return Quantite * PrixUnitaire
        End Get
    End Property
End Class
