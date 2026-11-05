Imports System.Security.Cryptography
Imports System.Text

' Outils de securite. NOTE : pour la production, preferer un hachage avec sel
' et cout (ex. BCrypt via le paquet NuGet BCrypt.Net-Next). SHA-256 est utilise
' ici pour rester sans dependance supplementaire dans cette premiere version.
Public Class SecurityHelper

    ' Retourne le hash hexadecimal SHA-256 d'un mot de passe.
    Public Shared Function HacherMotDePasse(motDePasse As String) As String
        If motDePasse Is Nothing Then motDePasse = String.Empty
        Using sha As SHA256 = SHA256.Create()
            Dim octets As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(motDePasse))
            Dim sb As New StringBuilder(octets.Length * 2)
            For Each b As Byte In octets
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

End Class
