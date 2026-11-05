Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' Enregistrement des ventes / factures (transaction), avec FEFO et ordonnancier.
Public Class VenteRepository

    ' Retourne le lot FEFO disponible (peremption la plus proche) pour un produit, ou Nothing.
    Public Function LotFefo(idProduit As Integer) As DataRow
        Dim dt As DataTable = Database.ExecuterTable(
            "SELECT id_lot, numero_lot, quantite, date_peremption FROM lot " &
            "WHERE id_produit = @id AND statut = 'actif' AND quantite > 0 " &
            "ORDER BY date_peremption LIMIT 1",
            New MySqlParameter("@id", idProduit))
        If dt.Rows.Count = 0 Then Return Nothing
        Return dt.Rows(0)
    End Function

    ' Enregistre une vente complete : en-tete, lignes, mouvements de sortie, et
    ' ordonnancier pour les substances reglementees. Retourne le numero de facture.
    Public Function Enregistrer(idClient As Integer?, idUtilisateur As Integer,
                                remise As Decimal, lignes As List(Of LigneVente)) As String
        Using cn = Database.CreerConnexion()
            cn.Open()
            Using tx = cn.BeginTransaction()
                Try
                    Dim total As Decimal = 0D
                    For Each l In lignes
                        total += l.Montant
                    Next
                    total -= remise
                    If total < 0D Then total = 0D

                    Dim idVente As Integer = DbUtil.ExecInsertId(cn, tx,
                        "INSERT INTO vente (id_client, id_utilisateur, remise, montant_total) VALUES (@c, @u, @r, @t)",
                        New MySqlParameter("@c", If(idClient.HasValue, CObj(idClient.Value), DBNull.Value)),
                        New MySqlParameter("@u", idUtilisateur),
                        New MySqlParameter("@r", remise),
                        New MySqlParameter("@t", total))

                    Dim numero As String = "FAC-" & idVente.ToString("D6")
                    DbUtil.ExecNonQuery(cn, tx, "UPDATE vente SET numero_facture = @n WHERE id_vente = @id",
                        New MySqlParameter("@n", numero),
                        New MySqlParameter("@id", idVente))

                    For Each l In lignes
                        Dim idLigne As Integer = DbUtil.ExecInsertId(cn, tx,
                            "INSERT INTO ligne_vente (id_vente, id_produit, id_lot, quantite, prix_unitaire, montant) " &
                            "VALUES (@v, @p, @lot, @q, @pu, @m)",
                            New MySqlParameter("@v", idVente),
                            New MySqlParameter("@p", l.IdProduit),
                            New MySqlParameter("@lot", l.IdLot),
                            New MySqlParameter("@q", l.Quantite),
                            New MySqlParameter("@pu", l.PrixUnitaire),
                            New MySqlParameter("@m", l.Montant))

                        ' Mouvement de sortie : le declencheur verifie le stock et decremente le lot.
                        DbUtil.ExecNonQuery(cn, tx,
                            "INSERT INTO mouvement_stock (id_lot, type_mouvement, quantite, id_utilisateur, motif, reference) " &
                            "VALUES (@lot, 'sortie', @q, @u, 'Vente', @ref)",
                            New MySqlParameter("@lot", l.IdLot),
                            New MySqlParameter("@q", l.Quantite),
                            New MySqlParameter("@u", idUtilisateur),
                            New MySqlParameter("@ref", "VENTE:" & idVente))

                        ' Substance reglementee : enregistrement obligatoire a l'ordonnancier.
                        If l.Reglemente Then
                            DbUtil.ExecNonQuery(cn, tx,
                                "INSERT INTO ordonnancier (id_produit, id_lot, id_ligne_vente, quantite, nom_prescripteur, nom_patient, id_utilisateur) " &
                                "VALUES (@p, @lot, @lv, @q, @presc, @pat, @u)",
                                New MySqlParameter("@p", l.IdProduit),
                                New MySqlParameter("@lot", l.IdLot),
                                New MySqlParameter("@lv", idLigne),
                                New MySqlParameter("@q", l.Quantite),
                                New MySqlParameter("@presc", l.Prescripteur),
                                New MySqlParameter("@pat", l.Patient),
                                New MySqlParameter("@u", idUtilisateur))
                        End If
                    Next

                    tx.Commit()
                    Return numero
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function

End Class
