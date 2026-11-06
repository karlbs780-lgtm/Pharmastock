Imports System.Windows.Forms

' Fenetre principale (conteneur MDI) affichee apres connexion.
Partial Public Class FrmPrincipal

    Private ReadOnly _utilisateur As Utilisateur

    Public Sub New(utilisateur As Utilisateur)
        InitializeComponent()
        _utilisateur = utilisateur
        lblStatut.Text = $"Connecté : {_utilisateur.Nom} ({_utilisateur.Role})"
        AppliquerDroits()
    End Sub

    ' Acces par role : l'employe vend, gere les clients et consulte le stock ;
    ' le catalogue, les fournisseurs, les receptions et l'ordonnancier sont
    ' reserves a l'administrateur (pharmacien titulaire).
    Private Sub AppliquerDroits()
        Dim admin As Boolean = _utilisateur.EstAdministrateur
        mnuProduits.Visible = admin
        mnuFournisseurs.Visible = admin
        mnuLivraison.Visible = admin
        mnuOrdonnancier.Visible = admin
        mnuReglementaire.Visible = admin
    End Sub

    Private Sub mnuProduits_Click(sender As Object, e As EventArgs) Handles mnuProduits.Click
        OuvrirEnfant(New FrmProduits())
    End Sub

    Private Sub mnuFournisseurs_Click(sender As Object, e As EventArgs) Handles mnuFournisseurs.Click
        OuvrirEnfant(New FrmFournisseurs())
    End Sub

    Private Sub mnuClients_Click(sender As Object, e As EventArgs) Handles mnuClients.Click
        OuvrirEnfant(New FrmClients())
    End Sub

    Private Sub mnuLivraison_Click(sender As Object, e As EventArgs) Handles mnuLivraison.Click
        OuvrirEnfant(New FrmLivraison(_utilisateur))
    End Sub

    Private Sub mnuVente_Click(sender As Object, e As EventArgs) Handles mnuVente.Click
        OuvrirEnfant(New FrmVente(_utilisateur))
    End Sub

    Private Sub mnuEtatStock_Click(sender As Object, e As EventArgs) Handles mnuEtatStock.Click
        OuvrirEnfant(New FrmStock())
    End Sub

    Private Sub mnuTableauBord_Click(sender As Object, e As EventArgs) Handles mnuTableauBord.Click
        OuvrirEnfant(New FrmTableauBord())
    End Sub

    Private Sub mnuOrdonnancier_Click(sender As Object, e As EventArgs) Handles mnuOrdonnancier.Click
        OuvrirEnfant(New FrmOrdonnancier())
    End Sub

    Private Sub mnuQuitter_Click(sender As Object, e As EventArgs) Handles mnuQuitter.Click
        Close()
    End Sub

    ' Ouvre un formulaire enfant MDI, ou l'active s'il est deja ouvert.
    Private Sub OuvrirEnfant(f As Form)
        For Each enfant As Form In Me.MdiChildren
            If enfant.GetType() Is f.GetType() Then
                enfant.Activate()
                f.Dispose()
                Return
            End If
        Next
        f.MdiParent = Me
        f.Show()
    End Sub

End Class
