Imports System.Data
Imports MySql.Data.MySqlClient

' Acces aux donnees des utilisateurs (authentification).
Public Class UtilisateurRepository

    ' Verifie les identifiants et retourne l'utilisateur, ou Nothing si echec.
    Public Function Authentifier(login As String, motDePasse As String) As Utilisateur
        Dim hash As String = SecurityHelper.HacherMotDePasse(motDePasse)

        Dim sql As String =
            "SELECT u.id_utilisateur, u.nom, u.login, u.id_role, r.libelle " &
            "FROM utilisateur u " &
            "JOIN role r ON r.id_role = u.id_role " &
            "WHERE u.login = @login AND u.mot_de_passe_hash = @hash AND u.actif = 1"

        Dim dt As DataTable = Database.ExecuterTable(
            sql,
            New MySqlParameter("@login", login),
            New MySqlParameter("@hash", hash))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)
        Return New Utilisateur With {
            .IdUtilisateur = Convert.ToInt32(r("id_utilisateur")),
            .Nom = Convert.ToString(r("nom")),
            .Login = Convert.ToString(r("login")),
            .IdRole = Convert.ToInt32(r("id_role")),
            .Role = Convert.ToString(r("libelle"))
        }
    End Function

End Class
