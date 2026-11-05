Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

' Consultation du stock et des alertes (vues SQL).
Public Class FrmStock
    Inherits Form

    Private ReadOnly _repo As New StockRepository()
    Private ReadOnly _grille As DataGridView

    Public Sub New()
        Text = "État du stock"
        ClientSize = New Size(840, 490)
        StartPosition = FormStartPosition.CenterParent

        Dim panneau As New Panel With {.Dock = DockStyle.Top, .Height = 44}
        Dim b1 As Button = Bouton("Stock par produit", 12)
        Dim b2 As Button = Bouton("Lots", 172)
        Dim b3 As Button = Bouton("Alertes rupture", 332)
        Dim b4 As Button = Bouton("Alertes péremption", 492)
        panneau.Controls.AddRange(New Control() {b1, b2, b3, b4})

        _grille = New DataGridView With {
            .Dock = DockStyle.Fill, .ReadOnly = True, .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False, .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .RowHeadersVisible = False}

        Controls.Add(_grille)
        Controls.Add(panneau)

        AddHandler b1.Click, Sub(s, e) Afficher(AddressOf _repo.StockParProduit)
        AddHandler b2.Click, Sub(s, e) Afficher(AddressOf _repo.Lots)
        AddHandler b3.Click, Sub(s, e) Afficher(AddressOf _repo.AlertesRupture)
        AddHandler b4.Click, Sub(s, e) Afficher(AddressOf _repo.AlertesPeremption)
        AddHandler Me.Load, Sub(s, e) Afficher(AddressOf _repo.StockParProduit)
    End Sub

    Private Function Bouton(texte As String, x As Integer) As Button
        Return New Button With {.Text = texte, .Location = New Point(x, 9), .Width = 150}
    End Function

    Private Sub Afficher(source As Func(Of DataTable))
        Try
            _grille.DataSource = source()
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
