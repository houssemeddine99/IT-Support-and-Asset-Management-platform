Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports ITSupportAssetManagement.Web.Models
Namespace Data
    Public NotInheritable Class AuditRepository
        Public Shared Sub Record(actionName As String, entityType As String, entityKey As String, summary As String)
            Try
                Dim context = HttpContext.Current, userId As Integer, actor As Integer? = Nothing
                If context IsNot Nothing AndAlso Integer.TryParse(Convert.ToString(context.Session("UserId")), userId) Then actor = userId
                Const sql = "INSERT dbo.AuditLogs(ActorUserId,ActionName,EntityType,EntityKey,Summary,IpAddress) VALUES(@Actor,@Action,@EntityType,@EntityKey,@Summary,@Ip);"
                Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                    command.Parameters.Add("@Actor", SqlDbType.Int).Value = If(actor.HasValue, CType(actor.Value, Object), DBNull.Value) : command.Parameters.Add("@Action", SqlDbType.NVarChar, 80).Value = actionName
                    command.Parameters.Add("@EntityType", SqlDbType.NVarChar, 80).Value = entityType : command.Parameters.Add("@EntityKey", SqlDbType.NVarChar, 80).Value = If(String.IsNullOrWhiteSpace(entityKey), CType(DBNull.Value, Object), entityKey)
                    command.Parameters.Add("@Summary", SqlDbType.NVarChar, 1000).Value = If(String.IsNullOrWhiteSpace(summary), actionName, summary)
                    Dim ip = If(context Is Nothing, String.Empty, Convert.ToString(context.Request.UserHostAddress)) : command.Parameters.Add("@Ip", SqlDbType.NVarChar, 64).Value = If(String.IsNullOrWhiteSpace(ip), CType(DBNull.Value, Object), ip)
                    connection.Open() : command.ExecuteNonQuery()
                End Using
            Catch ex As SqlException
            End Try
        End Sub
        Public Function GetLogs(search As String, entityType As String) As List(Of AuditLogItem)
            Dim sql = "SELECT l.AuditLogId,COALESCE(u.FirstName+N' '+u.LastName,N'System') ActorName,l.ActionName,l.EntityType,l.EntityKey,l.Summary,l.IpAddress,l.CreatedAtUtc FROM dbo.AuditLogs l LEFT JOIN dbo.Users u ON u.UserId=l.ActorUserId WHERE 1=1 "
            Dim results As New List(Of AuditLogItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand()
                command.Connection = connection
                If Not String.IsNullOrWhiteSpace(search) Then sql &= "AND (l.Summary LIKE @Search OR l.EntityKey LIKE @Search OR u.FirstName+N' '+u.LastName LIKE @Search) " : command.Parameters.Add("@Search", SqlDbType.NVarChar, 180).Value = "%" & search.Trim() & "%"
                If Not String.IsNullOrWhiteSpace(entityType) Then sql &= "AND l.EntityType=@EntityType " : command.Parameters.Add("@EntityType", SqlDbType.NVarChar, 80).Value = entityType
                command.CommandText = sql & "ORDER BY l.CreatedAtUtc DESC OFFSET 0 ROWS FETCH NEXT 250 ROWS ONLY;" : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New AuditLogItem With {.AuditLogId = reader.GetInt64(0), .ActorName = reader.GetString(1), .ActionName = reader.GetString(2), .EntityType = reader.GetString(3), .EntityKey = If(reader.IsDBNull(4), String.Empty, reader.GetString(4)), .Summary = reader.GetString(5), .IpAddress = If(reader.IsDBNull(6), String.Empty, reader.GetString(6)), .CreatedAtUtc = reader.GetDateTime(7)})
                    End While
                End Using
            End Using
            Return results
        End Function
    End Class
End Namespace
