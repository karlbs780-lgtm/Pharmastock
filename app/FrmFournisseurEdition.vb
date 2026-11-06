Imports System.Drawing
Imports System.Windows.Forms

' Ajout / modification d'un fournisseur.
Public Class FrmFournisseurEdition
    Inherits Form

    Private ReadOnly _edition As Fournisseur
    Public Property Fournisseur As Fournisseur

    Private ReadOnly _txtNom As TextBox
    Private ReadOnly _txtTel As TextBox
    Private ReadOnly _txtMail As TextBox
    Private ReadOnly _txtAdr As TextBox
    Private ReadOnly _chkAgree As CheckBox
    Private ReadOnly _chkActif As CheckBox

    Public Sub New()
        Text = "Fournisseur"
        ClientSize = New Size(390, 300)
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterParent
        MaximizeBox = False
        MinimizeBox = False

        _txtNom = Champ("Nom *", 20)
        _txtTel = Champ("Téléphone", 58)
        _txtMail = Champ("Email", 96)
        _txtAdr = Champ("Adresse", 134)

        _chkAgree = New CheckBox With {.Text = "Fournisseur agréé", .Location = New Point(150, 174), .AutoSize = True, .Checked = True}
        _chkActif = New CheckBox With {.Text = "Actif", .Location = New Point(150, 198), .AutoSize = True, .Checked = True}
        Dim btnOk As New Button With {.Text = "Enregistrer", .Location = New Point(150, 240), .Width = 110}
        Dim btnAnn As New Button With {.Text = "Annuler", .Location = New Point(270, 240), .Width = 100, .DialogResult = DialogResult.Cancel}

        Controls.AddRange(New Control() {_chkAgree, _chkActif, btnOk, btnAnn})
        AcceptButton = btnOk
        CancelButton = btnAnn
        AddHandler btnOk.Click, Sub(s, e) Valider()
    End Sub

    Public Sub New(fournisseurAModifier As Fournisseur)
        Me.New()
        _edition = fournisseurAModifier
        Text = "Modifier le fournisseur"
        _txtNom.Text = _edition.Nom
        _txtTel.Text = _edition.Telephone
        _txtMail.Text = _edition.Email
        _txtAdr.Text = _edition.Adresse
        _chkAgree.Checked = _edition.Agree
        _chkActif.Checked = _edition.Actif
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
        Dim f As New Fournisseur()
        If _edition IsNot Nothing Then f.IdFournisseur = _edition.IdFournisseur
        f.Nom = _txtNom.Text.Trim()
        f.Telephone = _txtTel.Text.Trim()
        f.Email = _txtMail.Text.Trim()
        f.Adresse = _txtAdr.Text.Trim()
        f.Agree = _chkAgree.Checked
        f.Actif = _chkActif.Checked
        Fournisseur = f
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
