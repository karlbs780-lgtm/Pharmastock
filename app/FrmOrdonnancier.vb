Imports System.Drawing
Imports System.Windows.Forms

' Consultation de l'ordonnancier (lecture seule).
Public Class FrmOrdonnancier
    Inherits Form

    Private ReadOnly _repo As New OrdonnancierRepository()
    Private ReadOnly _grille As DataGridView

    Public Sub New()
        Text = "Ordonnancier — substances réglementées"
        ClientSize = New Size(860, 480)
        StartPosition = FormStartPosition.CenterParent

        ' Largeur initiale = largeur du formulaire (bouton ancre a droite).
        Dim panneau As New Panel With {.Dock = DockStyle.Top, .Height = 44, .Width = ClientSize.Width}
        Dim lbl As New Label With {.Text = "Registre des délivrances (lecture seule, conservation 10 ans)",
                                   .Location = New Point(12, 13), .AutoSize = True}
        Dim btnActualiser As New Button With {.Text = "Actualiser", .Location = New Point(740, 9), .Width = 100, .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        panneau.Controls.Add(lbl)
        panneau.Controls.Add(btnActualiser)

        _grille = New DataGridView With {
            .Dock = DockStyle.Fill, .ReadOnly = True, .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False, .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .RowHeadersVisible = False}

        Controls.Add(_grille)
        Controls.Add(panneau)

        AddHandler btnActualiser.Click, Sub(s, e) Charger()
        AddHandler Me.Load, Sub(s, e) Charger()
    End Sub

    Private Sub Charger()
        Try
            _grille.DataSource = _repo.Lister()
            If _grille.Columns.Contains("id_ordonnancier") Then _grille.Columns("id_ordonnancier").Visible = False
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
