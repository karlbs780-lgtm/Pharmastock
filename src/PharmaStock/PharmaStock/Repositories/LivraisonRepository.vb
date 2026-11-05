Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

' Enregistrement des livraisons fournisseurs (transaction).
Public Class LivraisonRepository

    ' Enregistre une livraison et ses lignes. Chaque ligne cree/alimente un lot
    ' et genere un mouvement d'entree (le declencheur met a jour la quantite du lot).
    Public Function Enregistrer(idFournisseur As Integer, idCommande As Integer?, numeroBon As String,
                                dateLivraison As Date, idUtilisateur As Integer,
                                lignes As List(Of LigneLivraison)) As Integer
        ' Controles metier avant toute ecriture : pas de lot deja perime.
        For Each l In lignes
            If l.DatePeremption.Date <= Date.Today OrElse l.DatePeremption.Date <= dateLivraison.Date Then
                Throw New InvalidOperationException(
                    $"Lot {l.NumeroLot} refusé : date de péremption ({l.DatePeremption:d}) dépassée ou atteinte.")
            End If
        Next

        Using cn = Database.CreerConnexion()
            cn.Open()
            Using tx = cn.BeginTransaction()
                Try
                    ' RG-12 : reception uniquement aupres d'un fournisseur agree et actif.
                    Using cmd As New MySqlCommand(
                        "SELECT COUNT(*) FROM fournisseur WHERE id_fournisseur = @f AND agree = 1 AND actif = 1", cn, tx)
                        cmd.Parameters.AddWithValue("@f", idFournisseur)
                        If Convert.ToInt32(cmd.ExecuteScalar()) = 0 Then
                            Throw New InvalidOperationException("Fournisseur non agréé ou inactif : réception refusée.")
                        End If
                    End Using

                    Dim statut As String = "complete"
                    For Each l In lignes
                        If l.QuantiteConforme < l.QuantiteLivree Then statut = "partielle"
                    Next

                    Dim idLivraison As Integer = DbUtil.ExecInsertId(cn, tx,
                        "INSERT INTO livraison (id_fournisseur, id_commande, numero_bon_livraison, date_livraison, statut, id_utilisateur) " &
                        "VALUES (@f, @c, @bl, @d, @s, @u)",
                        New MySqlParameter("@f", idFournisseur),
                        New MySqlParameter("@c", If(idCommande.HasValue, CObj(idCommande.Value), DBNull.Value)),
                        New MySqlParameter("@bl", DbUtil.NullIfEmpty(numeroBon)),
                        New MySqlParameter("@d", dateLivraison),
                        New MySqlParameter("@s", statut),
                        New MySqlParameter("@u", idUtilisateur))

                    For Each l In lignes
                        Dim idLot As Integer = TrouverOuCreerLot(cn, tx, l, idFournisseur, dateLivraison)

                        DbUtil.ExecNonQuery(cn, tx,
                            "INSERT INTO ligne_livraison (id_livraison, id_produit, id_lot, numero_lot, date_peremption, quantite_livree, quantite_conforme, motif_ecart) " &
                            "VALUES (@liv, @p, @lot, @nl, @dp, @ql, @qc, @motif)",
                            New MySqlParameter("@liv", idLivraison),
                            New MySqlParameter("@p", l.IdProduit),
                            New MySqlParameter("@lot", idLot),
                            New MySqlParameter("@nl", l.NumeroLot),
                            New MySqlParameter("@dp", l.DatePeremption),
                            New MySqlParameter("@ql", l.QuantiteLivree),
                            New MySqlParameter("@qc", l.QuantiteConforme),
                            New MySqlParameter("@motif", DbUtil.NullIfEmpty(l.MotifEcart)))

                        If l.QuantiteConforme > 0 Then
                            DbUtil.ExecNonQuery(cn, tx,
                                "INSERT INTO mouvement_stock (id_lot, type_mouvement, quantite, id_utilisateur, motif, reference) " &
                                "VALUES (@lot, 'entree', @q, @u, 'Livraison', @ref)",
                                New MySqlParameter("@lot", idLot),
                                New MySqlParameter("@q", l.QuantiteConforme),
                                New MySqlParameter("@u", idUtilisateur),
                                New MySqlParameter("@ref", "LIVRAISON:" & idLivraison))
                        End If
                    Next

                    tx.Commit()
                    Return idLivraison
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function

    ' Retourne le lot existant (meme produit + meme numero) ou en cree un nouveau a 0.
    Private Function TrouverOuCreerLot(cn As MySqlConnection, tx As MySqlTransaction,
                                       l As LigneLivraison, idFournisseur As Integer, dateReception As Date) As Integer
        Using cmd As New MySqlCommand("SELECT id_lot FROM lot WHERE id_produit = @p AND numero_lot = @nl LIMIT 1", cn, tx)
            cmd.Parameters.AddRange(New MySqlParameter() {
                New MySqlParameter("@p", l.IdProduit),
                New MySqlParameter("@nl", l.NumeroLot)})
            Dim res As Object = cmd.ExecuteScalar()
            If res IsNot Nothing AndAlso Not (res Is DBNull.Value) Then
                Return Convert.ToInt32(res)
            End If
        End Using

        Return DbUtil.ExecInsertId(cn, tx,
            "INSERT INTO lot (id_produit, numero_lot, date_peremption, quantite, id_fournisseur, date_reception, statut) " &
            "VALUES (@p, @nl, @dp, 0, @f, @dr, 'actif')",
            New MySqlParameter("@p", l.IdProduit),
            New MySqlParameter("@nl", l.NumeroLot),
            New MySqlParameter("@dp", l.DatePeremption),
            New MySqlParameter("@f", idFournisseur),
            New MySqlParameter("@dr", dateReception))
    End Function

End Class
