Imports System.Configuration
Imports System.Data
Imports MySql.Data.MySqlClient

' Couche d'acces aux donnees : centralise la connexion MySQL et les methodes d'execution.
Public Class Database

    Private Shared ReadOnly ChaineConnexion As String =
        ConfigurationManager.ConnectionStrings("PharmaStock").ConnectionString

    ' Cree une nouvelle connexion (a ouvrir/fermer par l'appelant, ou via Using).
    Public Shared Function CreerConnexion() As MySqlConnection
        Return New MySqlConnection(ChaineConnexion)
    End Function

    ' Retourne le resultat d'une requete SELECT sous forme de DataTable.
    Public Shared Function ExecuterTable(sql As String, ParamArray parametres As MySqlParameter()) As DataTable
        Using cn = CreerConnexion()
            Using cmd As New MySqlCommand(sql, cn)
                If parametres IsNot Nothing AndAlso parametres.Length > 0 Then
                    cmd.Parameters.AddRange(parametres)
                End If
                Using da As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    ' Execute une requete INSERT / UPDATE / DELETE ; retourne le nombre de lignes affectees.
    Public Shared Function ExecuterNonQuery(sql As String, ParamArray parametres As MySqlParameter()) As Integer
        Using cn = CreerConnexion()
            cn.Open()
            Using cmd As New MySqlCommand(sql, cn)
                If parametres IsNot Nothing AndAlso parametres.Length > 0 Then
                    cmd.Parameters.AddRange(parametres)
                End If
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ' Execute une requete renvoyant une valeur unique (COUNT, derniere cle, etc.).
    Public Shared Function ExecuterScalaire(sql As String, ParamArray parametres As MySqlParameter()) As Object
        Using cn = CreerConnexion()
            cn.Open()
            Using cmd As New MySqlCommand(sql, cn)
                If parametres IsNot Nothing AndAlso parametres.Length > 0 Then
                    cmd.Parameters.AddRange(parametres)
                End If
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

End Class
