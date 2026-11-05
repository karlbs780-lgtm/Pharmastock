Imports System.Windows.Forms

' Ecran de gestion des produits : liste, recherche et operations CRUD.
Partial Public Class FrmProduits

    Private ReadOnly _repo As New ProduitRepository()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub FrmProduits_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Charger()
    End Sub

    Private Sub Charger()
        Try
            grilleProduits.DataSource = _repo.Lister(txtRecherche.Text.Trim())
            MettreEnFormeGrille()
        Catch ex As Exception
            MessageBox.Show("Erreur de chargement : " & ex.Message, "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MettreEnFormeGrille()
        RenommerColonne("id_produit", "Id", visible:=False)
        RenommerColonne("code_barres", "Code-barres")
        RenommerColonne("nom_commercial", "Nom commercial")
        RenommerColonne("dci", "DCI")
        RenommerColonne("forme", "Forme")
        RenommerColonne("dosage", "Dosage")
        RenommerColonne("numero_amm", "N° AMM", visible:=False)
        RenommerColonne("prix_achat", "Prix achat")
        RenommerColonne("prix_vente", "Prix vente")
        RenommerColonne("seuil_min", "Seuil min")
        RenommerColonne("statut_homologation", "Homologation")
        RenommerColonne("sur_ordonnance", "Ordonnance")
        RenommerColonne("actif", "Actif")
    End Sub

    Private Sub RenommerColonne(nom As String, entete As String, Optional visible As Boolean = True)
        If grilleProduits.Columns.Contains(nom) Then
            grilleProduits.Columns(nom).HeaderText = entete
            grilleProduits.Columns(nom).Visible = visible
        End If
    End Sub

    Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
        Charger()
    End Sub

    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        txtRecherche.Clear()
        Charger()
    End Sub

    Private Sub btnAjouter_Click(sender As Object, e As EventArgs) Handles btnAjouter.Click
        Using f As New FrmProduitEdition()
            If f.ShowDialog(Me) = DialogResult.OK Then
                Enregistrer(Sub() _repo.Inserer(f.Produit), "Produit ajouté.")
            End If
        End Using
    End Sub

    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
        Dim p As Produit = ProduitSelectionne()
        If p Is Nothing Then Return
        Using f As New FrmProduitEdition(p)
            If f.ShowDialog(Me) = DialogResult.OK Then
                Enregistrer(Sub() _repo.Modifier(f.Produit), "Produit modifié.")
            End If
        End Using
    End Sub

    Private Sub btnSupprimer_Click(sender As Object, e As EventArgs) Handles btnSupprimer.Click
        Dim p As Produit = ProduitSelectionne()
        If p Is Nothing Then Return
        Dim reponse = MessageBox.Show(
            $"Désactiver le produit « {p.NomCommercial} » ?", "PharmaStock",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If reponse = DialogResult.Yes Then
            Enregistrer(Sub() _repo.Desactiver(p.IdProduit), "Produit désactivé.")
        End If
    End Sub

    ' Execute une action d'ecriture puis recharge la grille, avec gestion d'erreur.
    Private Sub Enregistrer(action As Action, messageSucces As String)
        Try
            action()
            Charger()
            lblInfo.Text = messageSucces
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Construit un Produit a partir de la ligne selectionnee.
    Private Function ProduitSelectionne() As Produit
        If grilleProduits.CurrentRow Is Nothing Then
            MessageBox.Show("Veuillez sélectionner un produit.", "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return Nothing
        End If

        Dim r As DataGridViewRow = grilleProduits.CurrentRow
        Return New Produit With {
            .IdProduit = EntierCellule(r, "id_produit"),
            .CodeBarres = TexteCellule(r, "code_barres"),
            .NomCommercial = TexteCellule(r, "nom_commercial"),
            .Dci = TexteCellule(r, "dci"),
            .Forme = TexteCellule(r, "forme"),
            .Dosage = TexteCellule(r, "dosage"),
            .NumeroAmm = TexteCellule(r, "numero_amm"),
            .StatutHomologation = TexteCellule(r, "statut_homologation"),
            .SurOrdonnance = BooleenCellule(r, "sur_ordonnance"),
            .PrixAchat = DecimalCellule(r, "prix_achat"),
            .PrixVente = DecimalCellule(r, "prix_vente"),
            .SeuilMin = EntierCellule(r, "seuil_min"),
            .Actif = BooleenCellule(r, "actif")
        }
    End Function

    Private Function TexteCellule(r As DataGridViewRow, nom As String) As String
        Dim v = r.Cells(nom).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return String.Empty
        Return Convert.ToString(v)
    End Function

    Private Function EntierCellule(r As DataGridViewRow, nom As String) As Integer
        Dim v = r.Cells(nom).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0
        Return Convert.ToInt32(v)
    End Function

    Private Function DecimalCellule(r As DataGridViewRow, nom As String) As Decimal
        Dim v = r.Cells(nom).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0D
        Return Convert.ToDecimal(v)
    End Function

    Private Function BooleenCellule(r As DataGridViewRow, nom As String) As Boolean
        Dim v = r.Cells(nom).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return False
        Return Convert.ToBoolean(v)
    End Function

End Class
