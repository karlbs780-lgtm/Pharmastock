Imports System.Drawing
Imports System.Windows.Forms

Partial Class FrmLogin
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Friend WithEvents lblTitre As Label
    Friend WithEvents lblLogin As Label
    Friend WithEvents lblMotDePasse As Label
    Friend WithEvents txtLogin As TextBox
    Friend WithEvents txtMotDePasse As TextBox
    Friend WithEvents btnConnexion As Button
    Friend WithEvents lblMessage As Label

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.lblTitre = New Label()
        Me.lblLogin = New Label()
        Me.lblMotDePasse = New Label()
        Me.txtLogin = New TextBox()
        Me.txtMotDePasse = New TextBox()
        Me.btnConnexion = New Button()
        Me.lblMessage = New Label()
        Me.SuspendLayout()
        '
        Me.lblTitre.AutoSize = True
        Me.lblTitre.Font = New Font("Segoe UI", 14.0!, FontStyle.Bold)
        Me.lblTitre.ForeColor = Color.FromArgb(18, 92, 67)
        Me.lblTitre.Location = New Point(28, 20)
        Me.lblTitre.Text = "PharmaStock — Connexion"
        '
        Me.lblLogin.AutoSize = True
        Me.lblLogin.Location = New Point(28, 72)
        Me.lblLogin.Text = "Identifiant"
        '
        Me.txtLogin.Location = New Point(28, 92)
        Me.txtLogin.Size = New Size(324, 23)
        '
        Me.lblMotDePasse.AutoSize = True
        Me.lblMotDePasse.Location = New Point(28, 126)
        Me.lblMotDePasse.Text = "Mot de passe"
        '
        Me.txtMotDePasse.Location = New Point(28, 146)
        Me.txtMotDePasse.Size = New Size(324, 23)
        Me.txtMotDePasse.UseSystemPasswordChar = True
        '
        Me.btnConnexion.Location = New Point(28, 186)
        Me.btnConnexion.Size = New Size(324, 34)
        Me.btnConnexion.Text = "Se connecter"
        Me.btnConnexion.UseVisualStyleBackColor = True
        '
        Me.lblMessage.ForeColor = Color.Firebrick
        Me.lblMessage.Location = New Point(28, 232)
        ' Pas de MaximumSize (324, 0) : apres mise a l'echelle DPI la hauteur
        ' maximale devenait 0 et le message d'erreur etait invisible.
        Me.lblMessage.AutoSize = False
        Me.lblMessage.Size = New Size(324, 40)
        '
        Me.AcceptButton = Me.btnConnexion
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(380, 285)
        Me.Controls.Add(Me.lblTitre)
        Me.Controls.Add(Me.lblLogin)
        Me.Controls.Add(Me.txtLogin)
        Me.Controls.Add(Me.lblMotDePasse)
        Me.Controls.Add(Me.txtMotDePasse)
        Me.Controls.Add(Me.btnConnexion)
        Me.Controls.Add(Me.lblMessage)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "PharmaStock — Connexion"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

End Class
