Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

' Saisie d'une livraison fournisseur : en-tete + lignes, puis validation.
Public Class FrmLivraison
    Inherits Form

    Private ReadOnly _repoFournisseur As New FournisseurRepository()
    Private ReadOnly _repoProduit As New ProduitRepository()
    Private ReadOnly _repoLivraison As New LivraisonRepository()
    Private ReadOnly _utilisateur As Utilisateur
    Private ReadOnly _lignes As New List(Of LigneLivraison)()

    Private ReadOnly _cboFournisseur As New ComboBox()
    Private ReadOnly _txtBon As New TextBox()
    Private ReadOnly _dtpDate As New DateTimePicker()
    Private ReadOnly _cboProduit As New ComboBox()
    Private ReadOnly _txtLot As New TextBox()
    Private ReadOnly _dtpPeremption As New DateTimePicker()
    Private ReadOnly _numLivree As New NumericUpDown()
    Private ReadOnly _numConforme As New NumericUpDown()
    Private ReadOnly _txtMotif As New TextBox()
    Private ReadOnly _grille As New DataGridView()

    Public Sub New(utilisateur As Utilisateur)
        _utilisateur = utilisateur
        Text = "Nouvelle livraison fournisseur"
        ClientSize = New Size(860, 560)
        StartPosition = FormStartPosition.CenterParent
        ConstruireInterface()
        ChargerCombos()
    End Sub

    Private Sub ConstruireInterface()
        Dim panTop As New Panel With {.Dock = DockStyle.Top, .Height = 160}

        panTop.Controls.Add(New Label With {.Text = "Fournisseur", .Location = New Point(12, 15), .AutoSize = True})
        _cboFournisseur.SetBounds(95, 12, 240, 23)
        _cboFournisseur.DropDownStyle = ComboBoxStyle.DropDownList
        panTop.Controls.Add(_cboFournisseur)

        panTop.Controls.Add(New Label With {.Text = "N° bon", .Location = New Point(355, 15), .AutoSize = True})
        _txtBon.SetBounds(410, 12, 120, 23)
        panTop.Controls.Add(_txtBon)

        panTop.Controls.Add(New Label With {.Text = "Date", .Location = New Point(550, 15), .AutoSize = True})
        _dtpDate.SetBounds(590, 12, 130, 23)
        _dtpDate.Format = DateTimePickerFormat.Short
        panTop.Controls.Add(_dtpDate)

        Dim grp As New GroupBox With {.Text = "Ajouter une ligne", .Location = New Point(12, 48), .Size = New Size(824, 100)}

        grp.Controls.Add(New Label With {.Text = "Produit", .Location = New Point(12, 24), .AutoSize = True})
        _cboProduit.SetBounds(68, 21, 230, 23)
        _cboProduit.DropDownStyle = ComboBoxStyle.DropDownList
        grp.Controls.Add(_cboProduit)

        grp.Controls.Add(New Label With {.Text = "N° lot", .Location = New Point(312, 24), .AutoSize = True})
        _txtLot.SetBounds(356, 21, 110, 23)
        grp.Controls.Add(_txtLot)

        grp.Controls.Add(New Label With {.Text = "Péremption", .Location = New Point(480, 24), .AutoSize = True})
        _dtpPeremption.SetBounds(556, 21, 120, 23)
        _dtpPeremption.Format = DateTimePickerFormat.Short
        _dtpPeremption.Value = Date.Today.AddYears(1)
        grp.Controls.Add(_dtpPeremption)

        grp.Controls.Add(New Label With {.Text = "Qté livrée", .Location = New Point(12, 62), .AutoSize = True})
        _numLivree.SetBounds(88, 60, 70, 23)
        _numLivree.Maximum = 1000000D
        grp.Controls.Add(_numLivree)

        grp.Controls.Add(New Label With {.Text = "Qté conforme", .Location = New Point(172, 62), .AutoSize = True})
        _numConforme.SetBounds(262, 60, 70, 23)
        _numConforme.Maximum = 1000000D
        grp.Controls.Add(_numConforme)

        grp.Controls.Add(New Label With {.Text = "Motif écart", .Location = New Point(348, 62), .AutoSize = True})
        _txtMotif.SetBounds(420, 60, 190, 23)
        grp.Controls.Add(_txtMotif)

        Dim btnLigne As New Button With {.Text = "Ajouter la ligne", .Location = New Point(626, 58), .Width = 180, .Height = 27}
        grp.Controls.Add(btnLigne)
        panTop.Controls.Add(grp)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.MultiSelect = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.RowHeadersVisible = False

        ' Largeur initiale = largeur du formulaire (bouton ancre a droite).
        Dim panBas As New Panel With {.Dock = DockStyle.Bottom, .Height = 52, .Width = ClientSize.Width}
        Dim btnSupprLigne As New Button With {.Text = "Retirer la ligne", .Location = New Point(12, 10), .Width = 130}
        Dim btnValider As New Button With {.Text = "Valider la livraison", .Location = New Point(656, 9), .Width = 190, .Height = 32,
                                           .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        panBas.Controls.Add(btnSupprLigne)
        panBas.Controls.Add(btnValider)

        Controls.Add(_grille)
        Controls.Add(panBas)
        Controls.Add(panTop)

        AddHandler btnLigne.Click, Sub(s, e) AjouterLigne()
        AddHandler btnSupprLigne.Click, Sub(s, e) RetirerLigne()
        AddHandler btnValider.Click, Sub(s, e) Valider()
    End Sub

    Private Sub ChargerCombos()
        Try
            _cboFournisseur.DataSource = _repoFournisseur.ListerAgrees()
            _cboFournisseur.DisplayMember = "nom"
            _cboFournisseur.ValueMember = "id_fournisseur"

            _cboProduit.DataSource = _repoProduit.ListerActifs()
            _cboProduit.DisplayMember = "nom_commercial"
            _cboProduit.ValueMember = "id_produit"
        Catch ex As Exception
            MessageBox.Show("Erreur de chargement : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AjouterLigne()
        If _cboProduit.SelectedValue Is Nothing Then
            MessageBox.Show("Sélectionnez un produit.", "PharmaStock") : Return
        End If
        If _txtLot.Text.Trim().Length = 0 Then
            MessageBox.Show("Le numéro de lot est obligatoire.", "PharmaStock") : _txtLot.Focus() : Return
        End If
        Dim livree As Integer = Convert.ToInt32(_numLivree.Value)
        Dim conforme As Integer = Convert.ToInt32(_numConforme.Value)
        If livree <= 0 Then
            MessageBox.Show("La quantité livrée doit être supérieure à 0.", "PharmaStock") : Return
        End If
        If conforme < 0 OrElse conforme > livree Then
            MessageBox.Show("La quantité conforme doit être comprise entre 0 et la quantité livrée.", "PharmaStock") : Return
        End If
        If _dtpPeremption.Value.Date <= Date.Today OrElse _dtpPeremption.Value.Date <= _dtpDate.Value.Date Then
            MessageBox.Show("Lot périmé : la date de péremption doit être postérieure à aujourd'hui et à la date de livraison.",
                            "PharmaStock") : _dtpPeremption.Focus() : Return
        End If

        _lignes.Add(New LigneLivraison With {
            .IdProduit = Convert.ToInt32(_cboProduit.SelectedValue),
            .Produit = _cboProduit.Text,
            .NumeroLot = _txtLot.Text.Trim(),
            .DatePeremption = _dtpPeremption.Value.Date,
            .QuantiteLivree = livree,
            .QuantiteConforme = conforme,
            .MotifEcart = _txtMotif.Text.Trim()})

        RafraichirGrille()
        _txtLot.Clear()
        _txtMotif.Clear()
        _numLivree.Value = 0D
        _numConforme.Value = 0D
        _txtLot.Focus()
    End Sub

    Private Sub RetirerLigne()
        If _grille.CurrentRow Is Nothing Then Return
        Dim i As Integer = _grille.CurrentRow.Index
        If i >= 0 AndAlso i < _lignes.Count Then
            _lignes.RemoveAt(i)
            RafraichirGrille()
        End If
    End Sub

    Private Sub RafraichirGrille()
        _grille.DataSource = Nothing
        _grille.DataSource = New List(Of LigneLivraison)(_lignes)
        If _grille.Columns.Contains("IdProduit") Then _grille.Columns("IdProduit").Visible = False
    End Sub

    Private Sub Valider()
        If _cboFournisseur.SelectedValue Is Nothing Then
            MessageBox.Show("Sélectionnez un fournisseur.", "PharmaStock") : Return
        End If
        If _lignes.Count = 0 Then
            MessageBox.Show("Ajoutez au moins une ligne de livraison.", "PharmaStock") : Return
        End If

        Try
            Dim idLivraison As Integer = _repoLivraison.Enregistrer(
                Convert.ToInt32(_cboFournisseur.SelectedValue), Nothing, _txtBon.Text.Trim(),
                _dtpDate.Value.Date, _utilisateur.IdUtilisateur, _lignes)

            MessageBox.Show($"Livraison n° {idLivraison} enregistrée. Le stock a été mis à jour.",
                            "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Information)
            _lignes.Clear()
            RafraichirGrille()
            _txtBon.Clear()
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement : " & ex.Message, "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
