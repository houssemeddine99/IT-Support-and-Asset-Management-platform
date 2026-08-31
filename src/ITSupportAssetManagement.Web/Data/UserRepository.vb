Imports System.Data
Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class UserRepository
        Public Function AnyUsers() As Boolean
            Const sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Users) THEN 1 ELSE 0 END;"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                connection.Open()
                Return Convert.ToInt32(command.ExecuteScalar()) = 1
            End Using
        End Function

        Public Function FindActiveByEmail(email As String, ByRef passwordHash As String) As AuthenticatedUser
            Const sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, u.PasswordHash, r.Name AS RoleName " &
                        "FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId " &
                        "WHERE u.Email = @Email AND u.IsActive = 1;"

            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email.Trim()
                connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                    Return New AuthenticatedUser With {
                        .UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        .FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        .LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        .Email = reader.GetString(reader.GetOrdinal("Email")),
                        .RoleName = reader.GetString(reader.GetOrdinal("RoleName"))
                    }
                End Using
            End Using
        End Function

        Public Function CreateFirstAdministrator(firstName As String, lastName As String, email As String, passwordHash As String) As Integer
            Using connection = Database.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction(IsolationLevel.Serializable)
                    Try
                        Using countCommand = New SqlCommand("SELECT COUNT(1) FROM dbo.Users WITH (UPDLOCK, HOLDLOCK);", connection, transaction)
                            If Convert.ToInt32(countCommand.ExecuteScalar()) > 0 Then
                                Throw New InvalidOperationException("The initial administrator has already been created.")
                            End If
                        End Using

                        Const sql = "INSERT dbo.Users (RoleId, FirstName, LastName, Email, PasswordHash) " &
                                    "OUTPUT INSERTED.UserId " &
                                    "SELECT RoleId, @FirstName, @LastName, @Email, @PasswordHash FROM dbo.Roles WHERE Name = N'Administrator';"
                        Using command = New SqlCommand(sql, connection, transaction)
                            command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 80).Value = firstName.Trim()
                            command.Parameters.Add("@LastName", SqlDbType.NVarChar, 80).Value = lastName.Trim()
                            command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email.Trim().ToLowerInvariant()
                            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash
                            Dim result = command.ExecuteScalar()
                            If result Is Nothing Then Throw New InvalidOperationException("The Administrator role was not found. Run the database migration first.")
                            transaction.Commit()
                            Return Convert.ToInt32(result)
                        End Using
                    Catch
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Function

        Public Function GetActiveUsers() As List(Of LookupOption)
            Const sql = "SELECT UserId, FirstName + N' ' + LastName + N' — ' + COALESCE(Department, N'No department') AS Label FROM dbo.Users WHERE IsActive = 1 ORDER BY FirstName, LastName;"
            Dim results As New List(Of LookupOption)()
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New LookupOption With {.Id = reader.GetInt32(0), .Label = reader.GetString(1)})
                    End While
                End Using
            End Using
            Return results
        End Function
    End Class
End Namespace
