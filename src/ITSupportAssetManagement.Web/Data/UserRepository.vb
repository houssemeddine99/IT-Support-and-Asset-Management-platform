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

        Public Function GetActivePasswordHash(userId As Integer) As String
            Const sql = "SELECT PasswordHash FROM dbo.Users WHERE UserId=@UserId AND IsActive=1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId
                connection.Open()
                Return Convert.ToString(command.ExecuteScalar())
            End Using
        End Function

        Public Sub UpdatePassword(userId As Integer, passwordHash As String)
            Const sql = "UPDATE dbo.Users SET PasswordHash=@PasswordHash,UpdatedAtUtc=SYSUTCDATETIME() WHERE UserId=@UserId AND IsActive=1; IF @@ROWCOUNT=0 THROW 51000,'Active user not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash
                connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

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

        Public Function GetTeamMembers(search As String, roleName As String) As List(Of TeamMemberListItem)
            Dim sql = "SELECT u.UserId,u.EmployeeCode,u.FirstName,u.LastName,u.Email,u.Department,u.PhoneNumber,u.IsActive,r.Name RoleName FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId WHERE 1=1 "
            Dim results As New List(Of TeamMemberListItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand()
                command.Connection = connection
                If Not String.IsNullOrWhiteSpace(search) Then sql &= "AND (u.FirstName+N' '+u.LastName LIKE @Search OR u.Email LIKE @Search OR u.EmployeeCode LIKE @Search OR u.Department LIKE @Search) " : command.Parameters.Add("@Search", SqlDbType.NVarChar, 160).Value = "%" & search.Trim() & "%"
                If Not String.IsNullOrWhiteSpace(roleName) Then sql &= "AND r.Name=@Role " : command.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = roleName
                command.CommandText = sql & "ORDER BY u.IsActive DESC,u.FirstName,u.LastName;" : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New TeamMemberListItem With {.UserId = reader.GetInt32(0), .EmployeeCode = ReadString(reader, 1), .FirstName = reader.GetString(2), .LastName = reader.GetString(3), .Email = reader.GetString(4), .Department = ReadString(reader, 5), .PhoneNumber = ReadString(reader, 6), .IsActive = reader.GetBoolean(7), .RoleName = reader.GetString(8)})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetRoles() As List(Of LookupOption)
            Dim results As New List(Of LookupOption)()
            Using connection = Database.CreateConnection(), command As New SqlCommand("SELECT RoleId,Name FROM dbo.Roles ORDER BY CASE Name WHEN N'Administrator' THEN 1 WHEN N'ITManager' THEN 2 WHEN N'Technician' THEN 3 ELSE 4 END;", connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read() : results.Add(New LookupOption With {.Id = reader.GetInt32(0), .Label = reader.GetString(1)}) : End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetTeamMemberById(userId As Integer) As TeamMemberListItem
            Const sql = "SELECT u.UserId,u.RoleId,u.EmployeeCode,u.FirstName,u.LastName,u.Email,u.Department,u.PhoneNumber,u.IsActive,r.Name RoleName FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId WHERE u.UserId=@Id;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = userId : connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Return New TeamMemberListItem With {.UserId = reader.GetInt32(0), .RoleId = reader.GetInt32(1), .EmployeeCode = ReadString(reader, 2), .FirstName = reader.GetString(3), .LastName = reader.GetString(4), .Email = reader.GetString(5), .Department = ReadString(reader, 6), .PhoneNumber = ReadString(reader, 7), .IsActive = reader.GetBoolean(8), .RoleName = reader.GetString(9)}
                End Using
            End Using
        End Function

        Public Sub UpdateUser(userId As Integer, roleId As Integer, employeeCode As String, firstName As String, lastName As String, email As String, department As String, phoneNumber As String)
            If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(email) Then Throw New InvalidOperationException("Name and email are required.")
            Const sql = "UPDATE dbo.Users SET RoleId=@RoleId,EmployeeCode=@Code,FirstName=@FirstName,LastName=@LastName,Email=@Email,Department=@Department,PhoneNumber=@Phone,UpdatedAtUtc=SYSUTCDATETIME() WHERE UserId=@Id; IF @@ROWCOUNT=0 THROW 51000,'User not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = userId : command.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId : AddString(command, "@Code", 30, employeeCode)
                command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 80).Value = firstName.Trim() : command.Parameters.Add("@LastName", SqlDbType.NVarChar, 80).Value = lastName.Trim() : command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email.Trim().ToLowerInvariant()
                AddString(command, "@Department", 100, department) : AddString(command, "@Phone", 30, phoneNumber) : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Function CreateUser(roleId As Integer, employeeCode As String, firstName As String, lastName As String, email As String, passwordHash As String, department As String, phoneNumber As String) As Integer
            Const sql = "INSERT dbo.Users(RoleId,EmployeeCode,FirstName,LastName,Email,PasswordHash,Department,PhoneNumber) OUTPUT INSERTED.UserId VALUES(@RoleId,@Code,@FirstName,@LastName,@Email,@Hash,@Department,@Phone);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId : AddString(command, "@Code", 30, employeeCode)
                command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 80).Value = firstName.Trim() : command.Parameters.Add("@LastName", SqlDbType.NVarChar, 80).Value = lastName.Trim()
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email.Trim().ToLowerInvariant() : command.Parameters.Add("@Hash", SqlDbType.NVarChar, 500).Value = passwordHash
                AddString(command, "@Department", 100, department) : AddString(command, "@Phone", 30, phoneNumber)
                connection.Open() : Return Convert.ToInt32(command.ExecuteScalar())
            End Using
        End Function

        Public Sub SetUserActive(userId As Integer, isActive As Boolean)
            Const sql = "UPDATE dbo.Users SET IsActive=@IsActive,UpdatedAtUtc=SYSUTCDATETIME() WHERE UserId=@UserId; IF @@ROWCOUNT=0 THROW 51000,'The account was not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId : command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive
                connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub AddString(command As SqlCommand, name As String, length As Integer, value As String)
            command.Parameters.Add(name, SqlDbType.NVarChar, length).Value = If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value.Trim())
        End Sub
        Private Shared Function ReadString(reader As SqlDataReader, ordinal As Integer) As String
            Return If(reader.IsDBNull(ordinal), String.Empty, reader.GetString(ordinal))
        End Function
    End Class
End Namespace
