Imports MySql.Data.MySqlClient

' Petits utilitaires d'acces aux donnees partages par les depots.
Public Module DbUtil

    ' Convertit une chaine vide en NULL SQL.
    Public Function NullIfEmpty(valeur As String) As Object
        If String.IsNullOrWhiteSpace(valeur) Then Return DBNull.Value
        Return valeur
    End Function

    ' Execute une commande dans une transaction existante.
    Public Function ExecNonQuery(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cmd As New MySqlCommand(sql, cn, tx)
            If ps IsNot Nothing AndAlso ps.Length > 0 Then cmd.Parameters.AddRange(ps)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    ' Execute un INSERT et retourne l'identifiant genere.
    Public Function ExecInsertId(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cmd As New MySqlCommand(sql, cn, tx)
            If ps IsNot Nothing AndAlso ps.Length > 0 Then cmd.Parameters.AddRange(ps)
            cmd.ExecuteNonQuery()
            Return Convert.ToInt32(cmd.LastInsertedId)
        End Using
    End Function

End Module
