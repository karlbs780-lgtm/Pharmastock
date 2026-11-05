Imports System.Data
Imports MySql.Data.MySqlClient

' Acces aux donnees des clients.
Public Class ClientRepository

    Public Function Lister(Optional recherche As String = "") As DataTable
        Dim sql As String = "SELECT id_client, nom, telephone, adresse FROM client "
        If String.IsNullOrWhiteSpace(recherche) Then
            Return Database.ExecuterTable(sql & "ORDER BY nom")
        End If
        Return Database.ExecuterTable(sql & "WHERE nom LIKE @r ORDER BY nom",
                                      New MySqlParameter("@r", "%" & recherche & "%"))
    End Function

    Public Function ListerPourCombo() As DataTable
        Return Database.ExecuterTable("SELECT id_client, nom FROM client ORDER BY nom")
    End Function

    Public Function Inserer(c As Client) As Integer
        Return Database.ExecuterNonQuery(
            "INSERT INTO client (nom, telephone, adresse) VALUES (@n, @t, @a)",
            New MySqlParameter("@n", c.Nom),
            New MySqlParameter("@t", DbUtil.NullIfEmpty(c.Telephone)),
            New MySqlParameter("@a", DbUtil.NullIfEmpty(c.Adresse)))
    End Function

    Public Function Modifier(c As Client) As Integer
        Return Database.ExecuterNonQuery(
            "UPDATE client SET nom=@n, telephone=@t, adresse=@a WHERE id_client=@id",
            New MySqlParameter("@n", c.Nom),
            New MySqlParameter("@t", DbUtil.NullIfEmpty(c.Telephone)),
            New MySqlParameter("@a", DbUtil.NullIfEmpty(c.Adresse)),
            New MySqlParameter("@id", c.IdClient))
    End Function

End Class
