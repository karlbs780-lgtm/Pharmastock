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
    Friend WithEvents mnuStock As ToolStripMenuItem
    Friend WithEvents mnuVentes As ToolStripMenuItem
    Friend WithEvents mnuLivraisons As ToolStripMenuItem
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
        Me.mnuStock = New ToolStripMenuItem()
        Me.mnuVentes = New ToolStripMenuItem()
        Me.mnuLivraisons = New ToolStripMenuItem()
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
        Me.mnuGestion.DropDownItems.AddRange(New ToolStripItem() {Me.mnuProduits, Me.mnuStock, Me.mnuVentes, Me.mnuLivraisons})
        Me.mnuProduits.Text = "Produits"
        Me.mnuStock.Text = "Stock (à venir)"
        Me.mnuStock.Enabled = False
        Me.mnuVentes.Text = "Ventes / Facturation (à venir)"
        Me.mnuVentes.Enabled = False
        Me.mnuLivraisons.Text = "Livraisons (à venir)"
        Me.mnuLivraisons.Enabled = False
        '
        Me.menuPrincipal.Items.AddRange(New ToolStripItem() {Me.mnuFichier, Me.mnuGestion})
        Me.menuPrincipal.Location = New Point(0, 0)
        Me.menuPrincipal.Size = New Size(900, 24)
        '
        Me.lblStatut.Text = "Prêt"
        Me.barreStatut.Items.Add(Me.lblStatut)
        Me.barreStatut.Location = New Point(0, 428)
        Me.barreStatut.Size = New Size(900, 22)
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(900, 450)
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
