Imports System.Drawing
Imports System.Windows.Forms

Partial Class FrmPrincipal
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Friend WithEvents menuPrincipal As MenuStrip
    Friend WithEvents mnuFichier As ToolStripMenuItem
    Friend WithEvents mnuQuitter As ToolStripMenuItem
    Friend WithEvents mnuGestion As ToolStripMenuItem
    Friend WithEvents mnuProduits As ToolStripMenuItem
    Friend WithEvents mnuFournisseurs As ToolStripMenuItem
    Friend WithEvents mnuClients As ToolStripMenuItem
    Friend WithEvents mnuOperations As ToolStripMenuItem
    Friend WithEvents mnuLivraison As ToolStripMenuItem
    Friend WithEvents mnuVente As ToolStripMenuItem
    Friend WithEvents mnuStockMenu As ToolStripMenuItem
    Friend WithEvents mnuEtatStock As ToolStripMenuItem
    Friend WithEvents mnuTableauBord As ToolStripMenuItem
    Friend WithEvents mnuReglementaire As ToolStripMenuItem
    Friend WithEvents mnuOrdonnancier As ToolStripMenuItem
    Friend WithEvents barreStatut As StatusStrip
    Friend WithEvents lblStatut As ToolStripStatusLabel

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.menuPrincipal = New MenuStrip()
        Me.mnuFichier = New ToolStripMenuItem()
        Me.mnuQuitter = New ToolStripMenuItem()
        Me.mnuGestion = New ToolStripMenuItem()
        Me.mnuProduits = New ToolStripMenuItem()
        Me.mnuFournisseurs = New ToolStripMenuItem()
        Me.mnuClients = New ToolStripMenuItem()
        Me.mnuOperations = New ToolStripMenuItem()
        Me.mnuLivraison = New ToolStripMenuItem()
        Me.mnuVente = New ToolStripMenuItem()
        Me.mnuStockMenu = New ToolStripMenuItem()
        Me.mnuEtatStock = New ToolStripMenuItem()
        Me.mnuTableauBord = New ToolStripMenuItem()
        Me.mnuReglementaire = New ToolStripMenuItem()
        Me.mnuOrdonnancier = New ToolStripMenuItem()
        Me.barreStatut = New StatusStrip()
        Me.lblStatut = New ToolStripStatusLabel()
        Me.menuPrincipal.SuspendLayout()
        Me.barreStatut.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.mnuFichier.Text = "Fichier"
        Me.mnuFichier.DropDownItems.Add(Me.mnuQuitter)
        Me.mnuQuitter.Text = "Quitter"
        '
        Me.mnuGestion.Text = "Gestion"
        Me.mnuGestion.DropDownItems.AddRange(New ToolStripItem() {Me.mnuProduits, Me.mnuFournisseurs, Me.mnuClients})
        Me.mnuProduits.Text = "Produits"
        Me.mnuFournisseurs.Text = "Fournisseurs"
        Me.mnuClients.Text = "Clients"
        '
        Me.mnuOperations.Text = "Opérations"
        Me.mnuOperations.DropDownItems.AddRange(New ToolStripItem() {Me.mnuLivraison, Me.mnuVente})
        Me.mnuLivraison.Text = "Nouvelle livraison fournisseur"
        Me.mnuVente.Text = "Nouvelle vente"
        '
        Me.mnuStockMenu.Text = "Stock"
        Me.mnuStockMenu.DropDownItems.AddRange(New ToolStripItem() {Me.mnuEtatStock, Me.mnuTableauBord})
        Me.mnuEtatStock.Text = "État du stock"
        Me.mnuTableauBord.Text = "Tableau de bord (alertes)"
        '
        Me.mnuReglementaire.Text = "Réglementaire"
        Me.mnuReglementaire.DropDownItems.Add(Me.mnuOrdonnancier)
        Me.mnuOrdonnancier.Text = "Ordonnancier"
        '
        Me.menuPrincipal.Items.AddRange(New ToolStripItem() {Me.mnuFichier, Me.mnuGestion, Me.mnuOperations, Me.mnuStockMenu, Me.mnuReglementaire})
        Me.menuPrincipal.Location = New Point(0, 0)
        Me.menuPrincipal.Size = New Size(920, 24)
        '
        Me.lblStatut.Text = "Prêt"
        Me.barreStatut.Items.Add(Me.lblStatut)
        Me.barreStatut.Location = New Point(0, 478)
        Me.barreStatut.Size = New Size(920, 22)
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(920, 500)
        Me.Controls.Add(Me.barreStatut)
        Me.Controls.Add(Me.menuPrincipal)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.menuPrincipal
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "PharmaStock"
        Me.menuPrincipal.ResumeLayout(False)
        Me.menuPrincipal.PerformLayout()
        Me.barreStatut.ResumeLayout(False)
        Me.barreStatut.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

End Class
