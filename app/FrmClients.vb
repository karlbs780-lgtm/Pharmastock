Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

' Gestion des clients (liste + ajout / modification).
Public Class FrmClients
    Inherits Form

    Private ReadOnly _repo As New ClientRepository()
    Private ReadOnly _grille As DataGridView
    Private ReadOnly _txtRecherche As TextBox

    Public Sub New()
        Text = "Clients"
        ClientSize = New Size(720, 470)
        StartPosition = FormStartPosition.CenterParent

        Dim panneau As New Panel With {.Dock = DockStyle.Top, .Height = 46}
        _txtRecherche = New TextBox With {.Location = New Point(12, 12), .Width = 220}
        Dim btnRech As New Button With {.Text = "Rechercher", .Location = New Point(238, 11), .Width = 95}
        Dim btnAjout As New Button With {.Text = "Ajouter", .Location = New Point(440, 11), .Width = 90}
        Dim btnMod As New Button With {.Text = "Modifier", .Location = New Point(536, 11), .Width = 90}
        panneau.Controls.AddRange(New Control() {_txtRecherche, btnRech, btnAjout, btnMod})

        _grille = New DataGridView With {
            .Dock = DockStyle.Fill, .ReadOnly = True, .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False, .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .RowHeadersVisible = False}

        Controls.Add(_grille)
        Controls.Add(panneau)

        AddHandler btnRech.Click, Sub(s, e) Charger()
        AddHandler btnAjout.Click, Sub(s, e) Ajouter()
        AddHandler btnMod.Click, Sub(s, e) Modifier()
        AddHandler Me.Load, Sub(s, e) Charger()
    End Sub

    Private Sub Charger()
        Try
            _grille.DataSource = _repo.Lister(_txtRecherche.Text.Trim())
            If _grille.Columns.Contains("id_client") Then _grille.Columns("id_client").Visible = False
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Ajouter()
        Using f As New FrmClientEdition()
            If f.ShowDialog(Me) = DialogResult.OK Then Faire(Sub() _repo.Inserer(f.Client))
        End Using
    End Sub

    Private Sub Modifier()
        Dim c As Client = Selection()
        If c Is Nothing Then Return
        Using f As New FrmClientEdition(c)
            If f.ShowDialog(Me) = DialogResult.OK Then Faire(Sub() _repo.Modifier(f.Client))
        End Using
    End Sub

    Private Sub Faire(action As Action)
        Try
            action()
            Charger()
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function Selection() As Client
        If _grille.CurrentRow Is Nothing Then
            MessageBox.Show("Sélectionnez un client.", "PharmaStock")
            Return Nothing
        End If
        Dim r As DataGridViewRow = _grille.CurrentRow
        Return New Client With {
            .IdClient = Convert.ToInt32(r.Cells("id_client").Value),
            .Nom = Txt(r, "nom"),
            .Telephone = Txt(r, "telephone"),
            .Adresse = Txt(r, "adresse")}
    End Function

    Private Function Txt(r As DataGridViewRow, col As String) As String
        Dim v = r.Cells(col).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return String.Empty
        Return Convert.ToString(v)
    End Function

End Class
