' Represente un produit du catalogue.
Public Class Produit
    Public Property IdProduit As Integer
    Public Property CodeBarres As String
    Public Property NomCommercial As String
    Public Property Dci As String
    Public Property Forme As String
    Public Property Dosage As String
    Public Property NumeroAmm As String
    Public Property StatutHomologation As String = "homologue"
    Public Property SurOrdonnance As Boolean
    Public Property PrixAchat As Decimal
    Public Property PrixVente As Decimal
    Public Property SeuilMin As Integer
    Public Property Actif As Boolean = True
End Class
