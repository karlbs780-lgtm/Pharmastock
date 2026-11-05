Imports System.Windows.Forms

' Point d'entree de l'application PharmaStock.
Module Program

    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Ecran de connexion : on ne lance l'application que si l'authentification reussit.
        Using frmLogin As New FrmLogin()
            If frmLogin.ShowDialog() = DialogResult.OK Then
                Application.Run(New FrmPrincipal(frmLogin.UtilisateurConnecte))
            End If
        End Using
    End Sub

End Module
