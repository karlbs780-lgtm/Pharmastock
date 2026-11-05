Imports System.Drawing
Imports System.Windows.Forms

Partial Class FrmProduitEdition
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Friend WithEvents lblCodeBarres As Label
    Friend WithEvents txtCodeBarres As TextBox
    Friend WithEvents lblNom As Label
    Friend WithEvents txtNom As TextBox
    Friend WithEvents lblDci As Label
    Friend WithEvents txtDci As TextBox
    Friend WithEvents lblForme As Label
    Friend WithEvents txtForme As TextBox
    Friend WithEvents lblDosage As Label
    Friend WithEvents txtDosage As TextBox
    Friend WithEvents lblAmm As Label
    Friend WithEvents txtAmm As TextBox
    Friend WithEvents lblStatut As Label
    Friend WithEvents cboStatut As ComboBox
    Friend WithEvents lblPrixAchat As Label
    Friend WithEvents numPrixAchat As NumericUpDown
    Friend WithEvents lblPrixVente As Label
    Friend WithEvents numPrixVente As NumericUpDown
    Friend WithEvents lblSeuil As Label
    Friend WithEvents numSeuil As NumericUpDown
    Friend WithEvents lblTableau As Label
    Friend WithEvents cboTableau As ComboBox
    Friend WithEvents chkOrdonnance As CheckBox
    Friend WithEvents chkActif As CheckBox
    Friend WithEvents btnOk As Button
    Friend WithEvents btnAnnuler As Button

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.lblCodeBarres = New Label()
        Me.txtCodeBarres = New TextBox()
        Me.lblNom = New Label()
        Me.txtNom = New TextBox()
        Me.lblDci = New Label()
        Me.txtDci = New TextBox()
        Me.lblForme = New Label()
        Me.txtForme = New TextBox()
        Me.lblDosage = New Label()
        Me.txtDosage = New TextBox()
        Me.lblAmm = New Label()
        Me.txtAmm = New TextBox()
        Me.lblStatut = New Label()
        Me.cboStatut = New ComboBox()
        Me.lblPrixAchat = New Label()
        Me.numPrixAchat = New NumericUpDown()
        Me.lblPrixVente = New Label()
        Me.numPrixVente = New NumericUpDown()
        Me.lblSeuil = New Label()
        Me.numSeuil = New NumericUpDown()
        Me.lblTableau = New Label()
        Me.cboTableau = New ComboBox()
        Me.chkOrdonnance = New CheckBox()
        Me.chkActif = New CheckBox()
        Me.btnOk = New Button()
        Me.btnAnnuler = New Button()
        CType(Me.numPrixAchat, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numPrixVente, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numSeuil, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        Me.lblCodeBarres.AutoSize = True : Me.lblCodeBarres.Location = New Point(18, 20) : Me.lblCodeBarres.Text = "Code-barres"
        Me.txtCodeBarres.Location = New Point(150, 17) : Me.txtCodeBarres.Size = New Size(220, 23)
        '
        Me.lblNom.AutoSize = True : Me.lblNom.Location = New Point(18, 52) : Me.lblNom.Text = "Nom commercial *"
        Me.txtNom.Location = New Point(150, 49) : Me.txtNom.Size = New Size(220, 23)
        '
        Me.lblDci.AutoSize = True : Me.lblDci.Location = New Point(18, 84) : Me.lblDci.Text = "DCI"
        Me.txtDci.Location = New Point(150, 81) : Me.txtDci.Size = New Size(220, 23)
        '
        Me.lblForme.AutoSize = True : Me.lblForme.Location = New Point(18, 116) : Me.lblForme.Text = "Forme"
        Me.txtForme.Location = New Point(150, 113) : Me.txtForme.Size = New Size(220, 23)
        '
        Me.lblDosage.AutoSize = True : Me.lblDosage.Location = New Point(18, 148) : Me.lblDosage.Text = "Dosage"
        Me.txtDosage.Location = New Point(150, 145) : Me.txtDosage.Size = New Size(220, 23)
        '
        Me.lblAmm.AutoSize = True : Me.lblAmm.Location = New Point(18, 180) : Me.lblAmm.Text = "N° AMM"
        Me.txtAmm.Location = New Point(150, 177) : Me.txtAmm.Size = New Size(220, 23)
        '
        Me.lblStatut.AutoSize = True : Me.lblStatut.Location = New Point(18, 212) : Me.lblStatut.Text = "Homologation"
        Me.cboStatut.Location = New Point(150, 209) : Me.cboStatut.Size = New Size(220, 23)
        Me.cboStatut.DropDownStyle = ComboBoxStyle.DropDownList
        '
        Me.lblPrixAchat.AutoSize = True : Me.lblPrixAchat.Location = New Point(18, 244) : Me.lblPrixAchat.Text = "Prix d'achat (FCFA)"
        Me.numPrixAchat.Location = New Point(150, 242) : Me.numPrixAchat.Size = New Size(220, 23)
        Me.numPrixAchat.Maximum = 100000000D : Me.numPrixAchat.ThousandsSeparator = True
        '
        Me.lblPrixVente.AutoSize = True : Me.lblPrixVente.Location = New Point(18, 276) : Me.lblPrixVente.Text = "Prix de vente (FCFA)"
        Me.numPrixVente.Location = New Point(150, 274) : Me.numPrixVente.Size = New Size(220, 23)
        Me.numPrixVente.Maximum = 100000000D : Me.numPrixVente.ThousandsSeparator = True
        '
        Me.lblSeuil.AutoSize = True : Me.lblSeuil.Location = New Point(18, 308) : Me.lblSeuil.Text = "Seuil minimal"
        Me.numSeuil.Location = New Point(150, 306) : Me.numSeuil.Size = New Size(220, 23)
        Me.numSeuil.Maximum = 1000000D
        '
        Me.lblTableau.AutoSize = True : Me.lblTableau.Location = New Point(18, 340) : Me.lblTableau.Text = "Tableau (réglementé)"
        Me.cboTableau.Location = New Point(150, 338) : Me.cboTableau.Size = New Size(220, 23)
        Me.cboTableau.DropDownStyle = ComboBoxStyle.DropDownList
        '
        Me.chkOrdonnance.AutoSize = True : Me.chkOrdonnance.Location = New Point(150, 370) : Me.chkOrdonnance.Text = "Soumis à ordonnance"
        Me.chkActif.AutoSize = True : Me.chkActif.Location = New Point(150, 394) : Me.chkActif.Text = "Produit actif"
        '
        Me.btnOk.Location = New Point(150, 428) : Me.btnOk.Size = New Size(105, 32) : Me.btnOk.Text = "Enregistrer"
        Me.btnAnnuler.Location = New Point(265, 428) : Me.btnAnnuler.Size = New Size(105, 32) : Me.btnAnnuler.Text = "Annuler"
        Me.btnAnnuler.DialogResult = DialogResult.Cancel
        '
        Me.AcceptButton = Me.btnOk
        Me.CancelButton = Me.btnAnnuler
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(390, 480)
        Me.Controls.Add(Me.lblCodeBarres) : Me.Controls.Add(Me.txtCodeBarres)
        Me.Controls.Add(Me.lblNom) : Me.Controls.Add(Me.txtNom)
        Me.Controls.Add(Me.lblDci) : Me.Controls.Add(Me.txtDci)
        Me.Controls.Add(Me.lblForme) : Me.Controls.Add(Me.txtForme)
        Me.Controls.Add(Me.lblDosage) : Me.Controls.Add(Me.txtDosage)
        Me.Controls.Add(Me.lblAmm) : Me.Controls.Add(Me.txtAmm)
        Me.Controls.Add(Me.lblStatut) : Me.Controls.Add(Me.cboStatut)
        Me.Controls.Add(Me.lblPrixAchat) : Me.Controls.Add(Me.numPrixAchat)
        Me.Controls.Add(Me.lblPrixVente) : Me.Controls.Add(Me.numPrixVente)
        Me.Controls.Add(Me.lblSeuil) : Me.Controls.Add(Me.numSeuil)
        Me.Controls.Add(Me.lblTableau) : Me.Controls.Add(Me.cboTableau)
        Me.Controls.Add(Me.chkOrdonnance) : Me.Controls.Add(Me.chkActif)
        Me.Controls.Add(Me.btnOk) : Me.Controls.Add(Me.btnAnnuler)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Produit"
        CType(Me.numPrixAchat, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numPrixVente, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numSeuil, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

End Class
