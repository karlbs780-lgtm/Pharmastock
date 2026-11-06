Imports System.Drawing
Imports System.Windows.Forms

' Ajout / modification d'un client.
Public Class FrmClientEdition
    Inherits Form

    Private ReadOnly _edition As Client
    Public Property Client As Client

    Private ReadOnly _txtNom As TextBox
    Private ReadOnly _txtTel As TextBox
    Private ReadOnly _txtAdr As TextBox

    Public Sub New()
        Text = "Client"
        ClientSize = New Size(390, 210)
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterParent
        MaximizeBox = False
        MinimizeBox = False

        _txtNom = Champ("Nom *", 20)
        _txtTel = Champ("Téléphone", 58)
        _txtAdr = Champ("Adresse", 96)

        Dim btnOk As New Button With {.Text = "Enregistrer", .Location = New Point(150, 150), .Width = 110}
        Dim btnAnn As New Button With {.Text = "Annuler", .Location = New Point(270, 150), .Width = 100, .DialogResult = DialogResult.Cancel}
        Controls.AddRange(New Control() {btnOk, btnAnn})
        AcceptButton = btnOk
        CancelButton = btnAnn
        AddHandler btnOk.Click, Sub(s, e) Valider()
    End Sub

    Public Sub New(clientAModifier As Client)
        Me.New()
        _edition = clientAModifier
        Text = "Modifier le client"
        _txtNom.Text = _edition.Nom
        _txtTel.Text = _edition.Telephone
        _txtAdr.Text = _edition.Adresse
    End Sub

    Private Function Champ(libelle As String, y As Integer) As TextBox
        Dim l As New Label With {.Text = libelle, .Location = New Point(18, y + 3), .AutoSize = True}
        Dim t As New TextBox With {.Location = New Point(150, y), .Width = 220}
        Controls.Add(l)
        Controls.Add(t)
        Return t
    End Function

    Private Sub Valider()
        If _txtNom.Text.Trim().Length = 0 Then
            MessageBox.Show("Le nom est obligatoire.", "PharmaStock")
            _txtNom.Focus()
            Return
        End If
        Dim c As New Client()
        If _edition IsNot Nothing Then c.IdClient = _edition.IdClient
        c.Nom = _txtNom.Text.Trim()
        c.Telephone = _txtTel.Text.Trim()
        c.Adresse = _txtAdr.Text.Trim()
        Client = c
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
