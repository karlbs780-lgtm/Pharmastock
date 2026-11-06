Imports System.Drawing
Imports System.Windows.Forms

' Tableau de bord : alertes de rupture et de peremption.
Public Class FrmTableauBord
    Inherits Form

    Private ReadOnly _repo As New StockRepository()
    Private ReadOnly _grilleRupture As DataGridView
    Private ReadOnly _grillePeremption As DataGridView

    Public Sub New()
        Text = "Tableau de bord"
        ClientSize = New Size(840, 540)
        StartPosition = FormStartPosition.CenterParent

        ' Zone basse : peremptions (prend l'espace restant)
        Dim panBas As New Panel With {.Dock = DockStyle.Fill}
        Dim lblPer As New Label With {.Dock = DockStyle.Top, .Height = 26, .Text = "Lots périmant sous 90 jours",
                                      .Font = New Font("Segoe UI", 10.0!, FontStyle.Bold), .Padding = New Padding(6, 4, 0, 0)}
        _grillePeremption = Grille()
        panBas.Controls.Add(_grillePeremption)
        panBas.Controls.Add(lblPer)

        ' Zone haute : ruptures
        Dim panHaut As New Panel With {.Dock = DockStyle.Top, .Height = 240}
        Dim lblRup As New Label With {.Dock = DockStyle.Top, .Height = 26, .Text = "Ruptures de stock (stock ≤ seuil minimal)",
                                      .Font = New Font("Segoe UI", 10.0!, FontStyle.Bold), .Padding = New Padding(6, 4, 0, 0)}
        _grilleRupture = Grille()
        panHaut.Controls.Add(_grilleRupture)
        panHaut.Controls.Add(lblRup)

        Controls.Add(panBas)
        Controls.Add(panHaut)

        AddHandler Me.Load, Sub(s, e) Charger()
    End Sub

    Private Function Grille() As DataGridView
        Return New DataGridView With {
            .Dock = DockStyle.Fill, .ReadOnly = True, .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False, .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .RowHeadersVisible = False}
    End Function

    Private Sub Charger()
        Try
            _grilleRupture.DataSource = _repo.AlertesRupture()
            _grillePeremption.DataSource = _repo.AlertesPeremption()
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
