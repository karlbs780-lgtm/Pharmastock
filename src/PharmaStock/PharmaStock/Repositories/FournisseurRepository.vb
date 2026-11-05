Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Acces aux donnees des fournisseurs.
Public Class FournisseurRepository

    Public Function Lister(Optional recherche As String = "") As DataTable
        Dim sql As String = "SELECT id_fournisseur, nom, agree, telephone, email, adresse, actif FROM fournisseur "
        If String.IsNullOrWhiteSpace(recherche) Then
            Return Database.ExecuterTable(sql & "ORDER BY nom")
        End If
        Return Database.ExecuterTable(sql & "WHERE nom LIKE @r ORDER BY nom",
                                      New MySqlParameter("@r", "%" & recherche & "%"))
    End Function

    Public Function ListerActifs() As DataTable
        Return Database.ExecuterTable("SELECT id_fournisseur, nom FROM fournisseur WHERE actif = 1 ORDER BY nom")
    End Function

    Public Function Inserer(f As Fournisseur) As Integer
        Return Database.ExecuterNonQuery(
            "INSERT INTO fournisseur (nom, agree, telephone, email, adresse, actif) " &
            "VALUES (@nom, @agree, @tel, @mail, @adr, @actif)", Parametres(f))
    End Function

    Public Function Modifier(f As Fournisseur) As Integer
        Dim ps As New List(Of MySqlParameter)(Parametres(f))
        ps.Add(New MySqlParameter("@id", f.IdFournisseur))
        Return Database.ExecuterNonQuery(
            "UPDATE fournisseur SET nom=@nom, agree=@agree, telephone=@tel, email=@mail, " &
            "adresse=@adr, actif=@actif WHERE id_fournisseur=@id", ps.ToArray())
    End Function

    Public Function Desactiver(id As Integer) As Integer
        Return Database.ExecuterNonQuery("UPDATE fournisseur SET actif = 0 WHERE id_fournisseur = @id",
                                         New MySqlParameter("@id", id))
    End Function

    Private Function Parametres(f As Fournisseur) As MySqlParameter()
        Return New MySqlParameter() {
            New MySqlParameter("@nom", f.Nom),
            New MySqlParameter("@agree", f.Agree),
            New MySqlParameter("@tel", DbUtil.NullIfEmpty(f.Telephone)),
            New MySqlParameter("@mail", DbUtil.NullIfEmpty(f.Email)),
            New MySqlParameter("@adr", DbUtil.NullIfEmpty(f.Adresse)),
            New MySqlParameter("@actif", f.Actif)
        }
    End Function

End Class
