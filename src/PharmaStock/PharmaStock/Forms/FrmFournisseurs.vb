Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

' Gestion des fournisseurs (liste + CRUD).
Public Class FrmFournisseurs
    Inherits Form

    Private ReadOnly _repo As New FournisseurRepository()
    Private ReadOnly _grille As DataGridView
    Private ReadOnly _txtRecherche As TextBox

    Public Sub New()
        Text = "Fournisseurs"
        ClientSize = New Size(780, 470)
        StartPosition = FormStartPosition.CenterParent

        Dim panneau As New Panel With {.Dock = DockStyle.Top, .Height = 46}
        _txtRecherche = New TextBox With {.Location = New Point(12, 12), .Width = 220}
        Dim btnRech As New Button With {.Text = "Rechercher", .Location = New Point(238, 11), .Width = 95}
        Dim btnAjout As New Button With {.Text = "Ajouter", .Location = New Point(440, 11), .Width = 90}
        Dim btnMod As New Button With {.Text = "Modifier", .Location = New Point(536, 11), .Width = 90}
        Dim btnDes As New Button With {.Text = "Désactiver", .Location = New Point(632, 11), .Width = 100}
        panneau.Controls.AddRange(New Control() {_txtRecherche, btnRech, btnAjout, btnMod, btnDes})

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
        AddHandler btnDes.Click, Sub(s, e) Desactiver()
        AddHandler Me.Load, Sub(s, e) Charger()
    End Sub

    Private Sub Charger()
        Try
            _grille.DataSource = _repo.Lister(_txtRecherche.Text.Trim())
            If _grille.Columns.Contains("id_fournisseur") Then _grille.Columns("id_fournisseur").Visible = False
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Ajouter()
        Using f As New FrmFournisseurEdition()
            If f.ShowDialog(Me) = DialogResult.OK Then Faire(Sub() _repo.Inserer(f.Fournisseur))
        End Using
    End Sub

    Private Sub Modifier()
        Dim fr As Fournisseur = Selection()
        If fr Is Nothing Then Return
        Using f As New FrmFournisseurEdition(fr)
            If f.ShowDialog(Me) = DialogResult.OK Then Faire(Sub() _repo.Modifier(f.Fournisseur))
        End Using
    End Sub

    Private Sub Desactiver()
        Dim fr As Fournisseur = Selection()
        If fr Is Nothing Then Return
        If MessageBox.Show($"Désactiver « {fr.Nom} » ?", "PharmaStock",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Faire(Sub() _repo.Desactiver(fr.IdFournisseur))
        End If
    End Sub

    Private Sub Faire(action As Action)
        Try
            action()
            Charger()
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function Selection() As Fournisseur
        If _grille.CurrentRow Is Nothing Then
            MessageBox.Show("Sélectionnez un fournisseur.", "PharmaStock")
            Return Nothing
        End If
        Dim r As DataGridViewRow = _grille.CurrentRow
        Return New Fournisseur With {
            .IdFournisseur = Convert.ToInt32(r.Cells("id_fournisseur").Value),
            .Nom = Txt(r, "nom"),
            .Agree = Convert.ToBoolean(r.Cells("agree").Value),
            .Telephone = Txt(r, "telephone"),
            .Email = Txt(r, "email"),
            .Adresse = Txt(r, "adresse"),
            .Actif = Convert.ToBoolean(r.Cells("actif").Value)}
    End Function

    Private Function Txt(r As DataGridViewRow, col As String) As String
        Dim v = r.Cells(col).Value
        If v Is Nothing OrElse v Is DBNull.Value Then Return String.Empty
        Return Convert.ToString(v)
    End Function

End Class
