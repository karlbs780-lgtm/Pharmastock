Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

' Saisie d'une vente / facture. Le lot delivre est choisi automatiquement
' selon le FEFO ; les substances reglementees passent a l'ordonnancier.
Public Class FrmVente
    Inherits Form

    Private ReadOnly _repoClient As New ClientRepository()
    Private ReadOnly _repoProduit As New ProduitRepository()
    Private ReadOnly _repoVente As New VenteRepository()
    Private ReadOnly _utilisateur As Utilisateur
    Private ReadOnly _lignes As New List(Of LigneVente)()
    Private _produits As DataTable

    Private ReadOnly _cboClient As New ComboBox()
    Private ReadOnly _cboProduit As New ComboBox()
    Private ReadOnly _numQte As New NumericUpDown()
    Private ReadOnly _numRemise As New NumericUpDown()
    Private ReadOnly _lblTotal As New Label()
    Private ReadOnly _grille As New DataGridView()

    Public Sub New(utilisateur As Utilisateur)
        _utilisateur = utilisateur
        Text = "Nouvelle vente"
        ClientSize = New Size(820, 540)
        StartPosition = FormStartPosition.CenterParent
        ConstruireInterface()
        ChargerCombos()
    End Sub

    Private Sub ConstruireInterface()
        Dim panTop As New Panel With {.Dock = DockStyle.Top, .Height = 120}

        panTop.Controls.Add(New Label With {.Text = "Client", .Location = New Point(12, 16), .AutoSize = True})
        _cboClient.SetBounds(60, 13, 260, 23)
        _cboClient.DropDownStyle = ComboBoxStyle.DropDownList
        panTop.Controls.Add(_cboClient)

        Dim grp As New GroupBox With {.Text = "Ajouter une ligne", .Location = New Point(12, 44), .Size = New Size(792, 66)}
        grp.Controls.Add(New Label With {.Text = "Produit", .Location = New Point(12, 26), .AutoSize = True})
        _cboProduit.SetBounds(68, 23, 250, 23)
        _cboProduit.DropDownStyle = ComboBoxStyle.DropDownList
        grp.Controls.Add(_cboProduit)
        grp.Controls.Add(New Label With {.Text = "Quantité", .Location = New Point(335, 26), .AutoSize = True})
        _numQte.SetBounds(400, 23, 70, 23)
        _numQte.Maximum = 100000D
        grp.Controls.Add(_numQte)
        Dim btnLigne As New Button With {.Text = "Ajouter la ligne", .Location = New Point(500, 21), .Width = 170, .Height = 27}
        grp.Controls.Add(btnLigne)
        panTop.Controls.Add(grp)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.MultiSelect = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.RowHeadersVisible = False

        ' Largeur initiale = largeur du formulaire : sinon le bouton ancre a droite
        ' (ajoute alors que le panneau fait 200 px) se retrouve hors de la fenetre.
        Dim panBas As New Panel With {.Dock = DockStyle.Bottom, .Height = 56, .Width = ClientSize.Width}
        Dim btnSupprLigne As New Button With {.Text = "Retirer la ligne", .Location = New Point(12, 12), .Width = 130}
        panBas.Controls.Add(btnSupprLigne)
        panBas.Controls.Add(New Label With {.Text = "Remise (FCFA)", .Location = New Point(300, 16), .AutoSize = True})
        _numRemise.SetBounds(392, 13, 90, 23)
        _numRemise.Maximum = 100000000D
        panBas.Controls.Add(_numRemise)
        _lblTotal.SetBounds(500, 14, 140, 24)
        _lblTotal.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
        _lblTotal.Text = "Total : 0"
        panBas.Controls.Add(_lblTotal)
        Dim btnValider As New Button With {.Text = "Valider la vente", .Location = New Point(656, 11), .Width = 150, .Height = 32,
                                           .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        panBas.Controls.Add(btnValider)

        Controls.Add(_grille)
        Controls.Add(panBas)
        Controls.Add(panTop)

        AddHandler btnLigne.Click, Sub(s, e) AjouterLigne()
        AddHandler btnSupprLigne.Click, Sub(s, e) RetirerLigne()
        AddHandler btnValider.Click, Sub(s, e) Valider()
        AddHandler _numRemise.ValueChanged, Sub(s, e) MajTotal()
    End Sub

    Private Sub ChargerCombos()
        Try
            Dim clients As DataTable = _repoClient.ListerPourCombo()
            Dim passage As DataRow = clients.NewRow()
            passage("id_client") = 0
            passage("nom") = "— Client de passage —"
            clients.Rows.InsertAt(passage, 0)
            _cboClient.DataSource = clients
            _cboClient.DisplayMember = "nom"
            _cboClient.ValueMember = "id_client"

            _produits = _repoProduit.ListerActifs()
            _cboProduit.DataSource = _produits
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
        Dim idProduit As Integer = Convert.ToInt32(_cboProduit.SelectedValue)
        Dim qte As Integer = Convert.ToInt32(_numQte.Value)
        If qte <= 0 Then
            MessageBox.Show("La quantité doit être supérieure à 0.", "PharmaStock") : Return
        End If

        Dim infos As DataRow() = _produits.Select("id_produit = " & idProduit)
        If infos.Length = 0 Then Return
        Dim prix As Decimal = Convert.ToDecimal(infos(0)("prix_vente"))
        Dim reglemente As Boolean = Convert.ToBoolean(infos(0)("reglemente"))

        ' Repartition FEFO sur un ou plusieurs lots non perimes, en tenant compte
        ' des quantites deja reservees par les lignes en cours de saisie.
        Dim reserve As New Dictionary(Of Integer, Integer)()
        For Each l In _lignes
            If reserve.ContainsKey(l.IdLot) Then reserve(l.IdLot) += l.Quantite Else reserve(l.IdLot) = l.Quantite
        Next
        Dim repartition As List(Of Tuple(Of DataRow, Integer))
        Try
            repartition = _repoVente.RepartirFefo(idProduit, qte, reserve)
        Catch ex As InvalidOperationException
            MessageBox.Show(ex.Message, "PharmaStock") : Return
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "PharmaStock", MessageBoxButtons.OK, MessageBoxIcon.Error) : Return
        End Try

        Dim presc As String = String.Empty
        Dim pat As String = String.Empty
        If reglemente Then
            presc = Microsoft.VisualBasic.Interaction.InputBox("Nom du prescripteur :", "Substance réglementée").Trim()
            If presc.Length = 0 Then
                MessageBox.Show("Le prescripteur est obligatoire pour une substance réglementée.", "PharmaStock") : Return
            End If
            pat = Microsoft.VisualBasic.Interaction.InputBox("Nom du patient :", "Substance réglementée").Trim()
            If pat.Length = 0 Then
                MessageBox.Show("Le patient est obligatoire pour une substance réglementée.", "PharmaStock") : Return
            End If
        End If

        ' Une ligne de vente par lot utilise (tracabilite lot par lot).
        For Each part In repartition
            _lignes.Add(New LigneVente With {
                .IdProduit = idProduit,
                .Produit = _cboProduit.Text,
                .IdLot = Convert.ToInt32(part.Item1("id_lot")),
                .NumeroLot = Convert.ToString(part.Item1("numero_lot")),
                .Quantite = part.Item2,
                .PrixUnitaire = prix,
                .Reglemente = reglemente,
                .Prescripteur = presc,
                .Patient = pat})
        Next

        RafraichirGrille()
        _numQte.Value = 0D
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
        _grille.DataSource = New List(Of LigneVente)(_lignes)
        For Each col As String In New String() {"IdProduit", "IdLot", "Reglemente", "Prescripteur", "Patient"}
            If _grille.Columns.Contains(col) Then _grille.Columns(col).Visible = False
        Next
        MajTotal()
    End Sub

    Private Sub MajTotal()
        Dim total As Decimal = 0D
        For Each l In _lignes
            total += l.Montant
        Next
        total -= _numRemise.Value
        If total < 0D Then total = 0D
        _lblTotal.Text = "Total : " & total.ToString("N0") & " FCFA"
    End Sub

    Private Sub Valider()
        If _lignes.Count = 0 Then
            MessageBox.Show("Ajoutez au moins une ligne.", "PharmaStock") : Return
        End If

        Dim idClient As Integer? = Nothing
        If _cboClient.SelectedValue IsNot Nothing Then
            Dim idc As Integer = Convert.ToInt32(_cboClient.SelectedValue)
            If idc > 0 Then idClient = idc
        End If

        Try
            Dim numero As String = _repoVente.Enregistrer(idClient, _utilisateur.IdUtilisateur, _numRemise.Value, _lignes)
            MessageBox.Show($"Vente enregistrée. Facture {numero}.", "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            _lignes.Clear()
            _numRemise.Value = 0D
            RafraichirGrille()
        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'enregistrement : " & ex.Message, "PharmaStock",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
