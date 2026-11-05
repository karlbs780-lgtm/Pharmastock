Imports System.Windows.Forms

' Fenetre principale (conteneur MDI) affichee apres connexion.
Partial Public Class FrmPrincipal

    Private ReadOnly _utilisateur As Utilisateur

    Public Sub New(utilisateur As Utilisateur)
        InitializeComponent()
        _utilisateur = utilisateur
        lblStatut.Text = $"Connecté : {_utilisateur.Nom} ({_utilisateur.Role})"
    End Sub

    Private Sub mnuProduits_Click(sender As Object, e As EventArgs) Handles mnuProduits.Click
        OuvrirEnfant(New FrmProduits())
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
