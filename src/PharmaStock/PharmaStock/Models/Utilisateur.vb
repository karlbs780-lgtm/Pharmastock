' Represente un utilisateur connecte.
Public Class Utilisateur
    Public Property IdUtilisateur As Integer
    Public Property Nom As String
    Public Property Login As String
    Public Property IdRole As Integer
    Public Property Role As String

    Public ReadOnly Property EstAdministrateur As Boolean
        Get
            Return String.Equals(Role, "administrateur", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property
End Class
