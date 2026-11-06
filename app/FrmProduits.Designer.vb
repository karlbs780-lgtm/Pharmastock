Imports System.Drawing
Imports System.Windows.Forms

Partial Class FrmProduits
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Friend WithEvents panneauHaut As Panel
    Friend WithEvents txtRecherche As TextBox
    Friend WithEvents btnRechercher As Button
    Friend WithEvents btnActualiser As Button
    Friend WithEvents btnAjouter As Button
    Friend WithEvents btnModifier As Button
    Friend WithEvents btnSupprimer As Button
    Friend WithEvents grilleProduits As DataGridView
    Friend WithEvents barreBas As StatusStrip
    Friend WithEvents lblInfo As ToolStripStatusLabel

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.panneauHaut = New Panel()
        Me.txtRecherche = New TextBox()
        Me.btnRechercher = New Button()
        Me.btnActualiser = New Button()
        Me.btnAjouter = New Button()
        Me.btnModifier = New Button()
        Me.btnSupprimer = New Button()
        Me.grilleProduits = New DataGridView()
        Me.barreBas = New StatusStrip()
        Me.lblInfo = New ToolStripStatusLabel()
        Me.panneauHaut.SuspendLayout()
        CType(Me.grilleProduits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.barreBas.SuspendLayout()
        Me.SuspendLayout()
        '
        Me.panneauHaut.Dock = DockStyle.Top
        Me.panneauHaut.Height = 48
        Me.panneauHaut.Controls.Add(Me.txtRecherche)
        Me.panneauHaut.Controls.Add(Me.btnRechercher)
        Me.panneauHaut.Controls.Add(Me.btnActualiser)
        Me.panneauHaut.Controls.Add(Me.btnAjouter)
        Me.panneauHaut.Controls.Add(Me.btnModifier)
        Me.panneauHaut.Controls.Add(Me.btnSupprimer)
        '
        Me.txtRecherche.Location = New Point(12, 13)
        Me.txtRecherche.Size = New Size(220, 23)
        '
        Me.btnRechercher.Location = New Point(238, 12)
        Me.btnRechercher.Size = New Size(90, 25)
        Me.btnRechercher.Text = "Rechercher"
        '
        Me.btnActualiser.Location = New Point(334, 12)
        Me.btnActualiser.Size = New Size(90, 25)
        Me.btnActualiser.Text = "Actualiser"
        '
        Me.btnAjouter.Location = New Point(470, 12)
        Me.btnAjouter.Size = New Size(80, 25)
        Me.btnAjouter.Text = "Ajouter"
        '
        Me.btnModifier.Location = New Point(556, 12)
        Me.btnModifier.Size = New Size(80, 25)
        Me.btnModifier.Text = "Modifier"
        '
        Me.btnSupprimer.Location = New Point(642, 12)
        Me.btnSupprimer.Size = New Size(90, 25)
        Me.btnSupprimer.Text = "Désactiver"
        '
        Me.grilleProduits.Dock = DockStyle.Fill
        Me.grilleProduits.AllowUserToAddRows = False
        Me.grilleProduits.AllowUserToDeleteRows = False
        Me.grilleProduits.ReadOnly = True
        Me.grilleProduits.MultiSelect = False
        Me.grilleProduits.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.grilleProduits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.grilleProduits.RowHeadersVisible = False
        '
        Me.lblInfo.Text = ""
        Me.barreBas.Items.Add(Me.lblInfo)
        Me.barreBas.Location = New Point(0, 428)
        Me.barreBas.Size = New Size(784, 22)
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(784, 450)
        Me.Controls.Add(Me.grilleProduits)
        Me.Controls.Add(Me.barreBas)
        Me.Controls.Add(Me.panneauHaut)
        Me.Text = "Gestion des produits"
        Me.panneauHaut.ResumeLayout(False)
        Me.panneauHaut.PerformLayout()
        CType(Me.grilleProduits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.barreBas.ResumeLayout(False)
        Me.barreBas.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

End Class
