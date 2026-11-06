Imports System.Windows.Forms

' Ecran de connexion. Renvoie l'utilisateur authentifie via UtilisateurConnecte.
Partial Public Class FrmLogin

    Private ReadOnly _repo As New UtilisateurRepository()

    Public Property UtilisateurConnecte As Utilisateur

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnConnexion_Click(sender As Object, e As EventArgs) Handles btnConnexion.Click
        lblMessage.Text = String.Empty

        If txtLogin.Text.Trim().Length = 0 Then
            lblMessage.Text = "Veuillez saisir votre identifiant."
            txtLogin.Focus()
            Return
        End If

        Try
            Dim u As Utilisateur = _repo.Authentifier(txtLogin.Text.Trim(), txtMotDePasse.Text)
            If u Is Nothing Then
                lblMessage.Text = "Identifiant ou mot de passe incorrect."
                txtMotDePasse.Clear()
                txtMotDePasse.Focus()
                Return
            End If

            UtilisateurConnecte = u
            DialogResult = DialogResult.OK
            Close()

        Catch ex As Exception
            lblMessage.Text = "Erreur de connexion a la base : " & ex.Message
        End Try
    End Sub

End Class
