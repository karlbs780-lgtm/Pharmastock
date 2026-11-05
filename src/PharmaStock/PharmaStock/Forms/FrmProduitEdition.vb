Imports System.Windows.Forms

' Fenetre d'ajout / modification d'un produit.
' Produit contient le resultat une fois la saisie validee.
Partial Public Class FrmProduitEdition

    Private ReadOnly _edition As Produit

    Public Property Produit As Produit

    Public Sub New()
        InitializeComponent()
        cboStatut.Items.AddRange(New Object() {"homologue", "non_homologue", "retire"})
        cboStatut.SelectedIndex = 0
    End Sub

    Public Sub New(produitAModifier As Produit)
        Me.New()
        _edition = produitAModifier
    End Sub

    Private Sub FrmProduitEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _edition Is Nothing Then
            Me.Text = "Nouveau produit"
            chkActif.Checked = True
            Return
        End If

        Me.Text = "Modifier le produit"
        txtCodeBarres.Text = _edition.CodeBarres
        txtNom.Text = _edition.NomCommercial
        txtDci.Text = _edition.Dci
        txtForme.Text = _edition.Forme
        txtDosage.Text = _edition.Dosage
        txtAmm.Text = _edition.NumeroAmm

        Dim idx As Integer = cboStatut.Items.IndexOf(_edition.StatutHomologation)
        cboStatut.SelectedIndex = If(idx >= 0, idx, 0)

        numPrixAchat.Value = Borner(_edition.PrixAchat, numPrixAchat)
        numPrixVente.Value = Borner(_edition.PrixVente, numPrixVente)
        numSeuil.Value = Borner(_edition.SeuilMin, numSeuil)
        chkOrdonnance.Checked = _edition.SurOrdonnance
        chkActif.Checked = _edition.Actif
    End Sub

    Private Function Borner(valeur As Decimal, champ As NumericUpDown) As Decimal
        If valeur < champ.Minimum Then Return champ.Minimum
        If valeur > champ.Maximum Then Return champ.Maximum
        Return valeur
    End Function

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If txtNom.Text.Trim().Length = 0 Then
            MessageBox.Show("Le nom commercial est obligatoire.", "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNom.Focus()
            Return
        End If

        Dim p As New Produit()
        If _edition IsNot Nothing Then p.IdProduit = _edition.IdProduit
        p.CodeBarres = txtCodeBarres.Text.Trim()
        p.NomCommercial = txtNom.Text.Trim()
        p.Dci = txtDci.Text.Trim()
        p.Forme = txtForme.Text.Trim()
        p.Dosage = txtDosage.Text.Trim()
        p.NumeroAmm = txtAmm.Text.Trim()
        p.StatutHomologation = Convert.ToString(cboStatut.SelectedItem)
        p.PrixAchat = numPrixAchat.Value
        p.PrixVente = numPrixVente.Value
        p.SeuilMin = Convert.ToInt32(numSeuil.Value)
        p.SurOrdonnance = chkOrdonnance.Checked
        p.Actif = chkActif.Checked

        Me.Produit = p
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
