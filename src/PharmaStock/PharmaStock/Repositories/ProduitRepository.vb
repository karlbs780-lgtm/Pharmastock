Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Acces aux donnees des produits (CRUD).
Public Class ProduitRepository

    ' Liste les produits, avec filtre optionnel sur nom / DCI / code-barres.
    Public Function Lister(Optional recherche As String = "") As DataTable
        Dim sql As String =
            "SELECT id_produit, code_barres, nom_commercial, dci, forme, dosage, numero_amm, " &
            "prix_achat, prix_vente, seuil_min, statut_homologation, sur_ordonnance, actif " &
            "FROM produit "

        If String.IsNullOrWhiteSpace(recherche) Then
            sql &= "ORDER BY nom_commercial"
            Return Database.ExecuterTable(sql)
        End If

        sql &= "WHERE nom_commercial LIKE @r OR dci LIKE @r OR code_barres LIKE @r ORDER BY nom_commercial"
        Return Database.ExecuterTable(sql, New MySqlParameter("@r", "%" & recherche & "%"))
    End Function

    ' Liste les produits actifs pour les ecrans de vente (prix + indicateur reglemente).
    Public Function ListerActifs() As DataTable
        Return Database.ExecuterTable(
            "SELECT id_produit, nom_commercial, prix_vente, (id_tableau IS NOT NULL) AS reglemente " &
            "FROM produit WHERE actif = 1 ORDER BY nom_commercial")
    End Function

    ' Insere un produit ; retourne le nombre de lignes affectees.
    Public Function Inserer(p As Produit) As Integer
        Dim sql As String =
            "INSERT INTO produit (code_barres, nom_commercial, dci, forme, dosage, numero_amm, " &
            "statut_homologation, sur_ordonnance, prix_achat, prix_vente, seuil_min, actif) " &
            "VALUES (@cb, @nom, @dci, @forme, @dosage, @amm, @statut, @ord, @pa, @pv, @seuil, @actif)"
        Return Database.ExecuterNonQuery(sql, Parametres(p))
    End Function

    ' Met a jour un produit existant.
    Public Function Modifier(p As Produit) As Integer
        Dim sql As String =
            "UPDATE produit SET code_barres=@cb, nom_commercial=@nom, dci=@dci, forme=@forme, " &
            "dosage=@dosage, numero_amm=@amm, statut_homologation=@statut, sur_ordonnance=@ord, " &
            "prix_achat=@pa, prix_vente=@pv, seuil_min=@seuil, actif=@actif WHERE id_produit=@id"

        Dim ps As New List(Of MySqlParameter)(Parametres(p))
        ps.Add(New MySqlParameter("@id", p.IdProduit))
        Return Database.ExecuterNonQuery(sql, ps.ToArray())
    End Function

    ' Suppression logique : on desactive le produit pour preserver l'historique.
    Public Function Desactiver(idProduit As Integer) As Integer
        Return Database.ExecuterNonQuery(
            "UPDATE produit SET actif = 0 WHERE id_produit = @id",
            New MySqlParameter("@id", idProduit))
    End Function

    ' Construit les parametres communs a l'insertion et a la mise a jour.
    Private Function Parametres(p As Produit) As MySqlParameter()
        Return New MySqlParameter() {
            New MySqlParameter("@cb", ValeurOuNull(p.CodeBarres)),
            New MySqlParameter("@nom", p.NomCommercial),
            New MySqlParameter("@dci", ValeurOuNull(p.Dci)),
            New MySqlParameter("@forme", ValeurOuNull(p.Forme)),
            New MySqlParameter("@dosage", ValeurOuNull(p.Dosage)),
            New MySqlParameter("@amm", ValeurOuNull(p.NumeroAmm)),
            New MySqlParameter("@statut", p.StatutHomologation),
            New MySqlParameter("@ord", p.SurOrdonnance),
            New MySqlParameter("@pa", p.PrixAchat),
            New MySqlParameter("@pv", p.PrixVente),
            New MySqlParameter("@seuil", p.SeuilMin),
            New MySqlParameter("@actif", p.Actif)
        }
    End Function

    ' Convertit une chaine vide en NULL SQL.
    Private Function ValeurOuNull(valeur As String) As Object
        If String.IsNullOrWhiteSpace(valeur) Then Return DBNull.Value
        Return valeur
    End Function

End Class
