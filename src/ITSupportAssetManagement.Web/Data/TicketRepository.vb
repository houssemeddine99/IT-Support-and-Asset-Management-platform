Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class TicketRepository
        Public Function GetTickets(search As String, status As String, priority As String, viewerUserId As Integer, canViewAll As Boolean) As List(Of TicketListItem)
            Dim sql = New StringBuilder(
                "SELECT t.TicketId, t.TicketNumber, t.Title, t.Priority, t.Status, c.Name AS CategoryName, " &
                "requester.FirstName + N' ' + requester.LastName AS RequestedByName, " &
                "CASE WHEN assignee.UserId IS NULL THEN NULL ELSE assignee.FirstName + N' ' + assignee.LastName END AS AssignedToName, " &
                "a.AssetTag, t.CreatedAtUtc " &
                "FROM dbo.Tickets t " &
                "INNER JOIN dbo.TicketCategories c ON c.TicketCategoryId = t.TicketCategoryId " &
                "INNER JOIN dbo.Users requester ON requester.UserId = t.RequestedByUserId " &
                "LEFT JOIN dbo.Users assignee ON assignee.UserId = t.AssignedToUserId " &
                "LEFT JOIN dbo.Assets a ON a.AssetId = t.AssetId WHERE 1 = 1 ")

            Dim results As New List(Of TicketListItem)()
            Using connection = Database.CreateConnection(), command = New SqlCommand()
                command.Connection = connection
                If Not canViewAll Then
                    sql.Append("AND t.RequestedByUserId = @ViewerUserId ")
                    command.Parameters.Add("@ViewerUserId", SqlDbType.Int).Value = viewerUserId
                End If
                If Not String.IsNullOrWhiteSpace(search) Then
                    sql.Append("AND (t.TicketNumber LIKE @Search OR t.Title LIKE @Search OR requester.FirstName + N' ' + requester.LastName LIKE @Search) ")
                    command.Parameters.Add("@Search", SqlDbType.NVarChar, 190).Value = "%" & search.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(status) Then
                    sql.Append("AND t.Status = @Status ")
                    command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status
                End If
                If Not String.IsNullOrWhiteSpace(priority) Then
                    sql.Append("AND t.Priority = @Priority ")
                    command.Parameters.Add("@Priority", SqlDbType.NVarChar, 20).Value = priority
                End If
                sql.Append("ORDER BY CASE t.Priority WHEN N'Critical' THEN 1 WHEN N'High' THEN 2 WHEN N'Medium' THEN 3 ELSE 4 END, t.CreatedAtUtc DESC;")
                command.CommandText = sql.ToString()
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New TicketListItem With {
                            .TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                            .TicketNumber = reader.GetString(reader.GetOrdinal("TicketNumber")),
                            .Title = reader.GetString(reader.GetOrdinal("Title")),
                            .Priority = reader.GetString(reader.GetOrdinal("Priority")),
                            .Status = reader.GetString(reader.GetOrdinal("Status")),
                            .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                            .RequestedByName = reader.GetString(reader.GetOrdinal("RequestedByName")),
                            .AssignedToName = ReadNullableString(reader, "AssignedToName"),
                            .AssetTag = ReadNullableString(reader, "AssetTag"),
                            .CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                        })
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetActiveCategories() As List(Of LookupOption)
            Return ReadOptions("SELECT TicketCategoryId, Name FROM dbo.TicketCategories WHERE IsActive = 1 ORDER BY Name;", "TicketCategoryId", "Name")
        End Function

        Public Function GetTicketById(ticketId As Integer, viewerUserId As Integer, canViewAll As Boolean) As TicketDetails
            Const sql = "SELECT t.TicketId, t.TicketNumber, t.Title, t.Description, t.Priority, t.Status, c.Name AS CategoryName, " &
                        "requester.FirstName + N' ' + requester.LastName AS RequestedByName, requester.Email AS RequestedByEmail, " &
                        "CASE WHEN assignee.UserId IS NULL THEN NULL ELSE assignee.FirstName + N' ' + assignee.LastName END AS AssignedToName, " &
                        "CASE WHEN a.AssetId IS NULL THEN NULL ELSE a.AssetTag + N' — ' + a.Model END AS AssetLabel, t.CreatedAtUtc, t.DueAtUtc " &
                        "FROM dbo.Tickets t INNER JOIN dbo.TicketCategories c ON c.TicketCategoryId = t.TicketCategoryId " &
                        "INNER JOIN dbo.Users requester ON requester.UserId = t.RequestedByUserId " &
                        "LEFT JOIN dbo.Users assignee ON assignee.UserId = t.AssignedToUserId LEFT JOIN dbo.Assets a ON a.AssetId = t.AssetId " &
                        "WHERE t.TicketId = @TicketId AND (@CanViewAll = 1 OR t.RequestedByUserId = @ViewerUserId);"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId
                command.Parameters.Add("@ViewerUserId", SqlDbType.Int).Value = viewerUserId
                command.Parameters.Add("@CanViewAll", SqlDbType.Bit).Value = canViewAll
                connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Dim dueAtOrdinal = reader.GetOrdinal("DueAtUtc")
                    Return New TicketDetails With {
                        .TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")), .TicketNumber = reader.GetString(reader.GetOrdinal("TicketNumber")),
                        .Title = reader.GetString(reader.GetOrdinal("Title")), .Description = reader.GetString(reader.GetOrdinal("Description")),
                        .Priority = reader.GetString(reader.GetOrdinal("Priority")), .Status = reader.GetString(reader.GetOrdinal("Status")),
                        .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")), .RequestedByName = reader.GetString(reader.GetOrdinal("RequestedByName")),
                        .RequestedByEmail = reader.GetString(reader.GetOrdinal("RequestedByEmail")), .AssignedToName = ReadNullableString(reader, "AssignedToName"),
                        .AssetLabel = ReadNullableString(reader, "AssetLabel"), .CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                        .DueAtUtc = If(reader.IsDBNull(dueAtOrdinal), CType(Nothing, DateTime?), reader.GetDateTime(dueAtOrdinal))
                    }
                End Using
            End Using
        End Function

        Public Function GetAvailableAssets() As List(Of LookupOption)
            Const sql = "SELECT AssetId, AssetTag + N' — ' + Model AS Label FROM dbo.Assets WHERE Status <> N'Retired' ORDER BY AssetTag;"
            Return ReadOptions(sql, "AssetId", "Label")
        End Function

        Public Function CreateTicket(categoryId As Integer, assetId As Integer?, requestedByUserId As Integer, title As String, description As String, priority As String) As String
            Const sql = "INSERT dbo.Tickets (TicketCategoryId, AssetId, RequestedByUserId, Title, Description, Priority) " &
                        "OUTPUT INSERTED.TicketNumber VALUES (@CategoryId, @AssetId, @RequestedByUserId, @Title, @Description, @Priority);"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = If(assetId.HasValue, CType(assetId.Value, Object), DBNull.Value)
                command.Parameters.Add("@RequestedByUserId", SqlDbType.Int).Value = requestedByUserId
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 180).Value = title.Trim()
                command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = description.Trim()
                command.Parameters.Add("@Priority", SqlDbType.NVarChar, 20).Value = priority
                connection.Open()
                Return Convert.ToString(command.ExecuteScalar())
            End Using
        End Function

        Public Function GetAssignableTechnicians() As List(Of LookupOption)
            Const sql = "SELECT u.UserId,u.FirstName+N' '+u.LastName+N' - '+r.Name Label FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId WHERE u.IsActive=1 AND r.Name IN(N'Administrator',N'ITManager',N'Technician') ORDER BY u.FirstName,u.LastName;"
            Return ReadOptions(sql, "UserId", "Label")
        End Function

        Public Function GetComments(ticketId As Integer, includeInternal As Boolean) As List(Of TicketCommentItem)
            Const sql = "SELECT u.FirstName+N' '+u.LastName AuthorName,c.Body,c.IsInternal,c.CreatedAtUtc FROM dbo.TicketComments c INNER JOIN dbo.Users u ON u.UserId=c.AuthorUserId WHERE c.TicketId=@TicketId AND (@IncludeInternal=1 OR c.IsInternal=0) ORDER BY c.CreatedAtUtc;"
            Dim results As New List(Of TicketCommentItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@IncludeInternal", SqlDbType.Bit).Value = includeInternal : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read() : results.Add(New TicketCommentItem With {.AuthorName = reader.GetString(0), .Body = reader.GetString(1), .IsInternal = reader.GetBoolean(2), .CreatedAtUtc = reader.GetDateTime(3)}) : End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetAttachments(ticketId As Integer, viewerUserId As Integer, canViewAll As Boolean) As List(Of TicketAttachmentItem)
            Const sql = "SELECT a.TicketAttachmentId,a.FileName,a.ContentType,a.FileSizeBytes,u.FirstName+N' '+u.LastName UploadedByName,a.CreatedAtUtc FROM dbo.TicketAttachments a INNER JOIN dbo.Tickets t ON t.TicketId=a.TicketId INNER JOIN dbo.Users u ON u.UserId=a.UploadedByUserId WHERE a.TicketId=@TicketId AND (@CanViewAll=1 OR t.RequestedByUserId=@UserId) ORDER BY a.CreatedAtUtc DESC;"
            Dim results As New List(Of TicketAttachmentItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@UserId", SqlDbType.Int).Value = viewerUserId : command.Parameters.Add("@CanViewAll", SqlDbType.Bit).Value = canViewAll : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New TicketAttachmentItem With {.TicketAttachmentId = reader.GetInt32(0), .FileName = reader.GetString(1), .ContentType = reader.GetString(2), .FileSizeBytes = reader.GetInt32(3), .UploadedByName = reader.GetString(4), .CreatedAtUtc = reader.GetDateTime(5)})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetAttachment(attachmentId As Integer, viewerUserId As Integer, canViewAll As Boolean) As TicketAttachmentItem
            Const sql = "SELECT a.FileName,a.ContentType,a.FileSizeBytes,a.FileContent FROM dbo.TicketAttachments a INNER JOIN dbo.Tickets t ON t.TicketId=a.TicketId WHERE a.TicketAttachmentId=@Id AND (@CanViewAll=1 OR t.RequestedByUserId=@UserId);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = attachmentId : command.Parameters.Add("@UserId", SqlDbType.Int).Value = viewerUserId : command.Parameters.Add("@CanViewAll", SqlDbType.Bit).Value = canViewAll : connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Return New TicketAttachmentItem With {.FileName = reader.GetString(0), .ContentType = reader.GetString(1), .FileSizeBytes = reader.GetInt32(2), .FileContent = DirectCast(reader(3), Byte())}
                End Using
            End Using
        End Function

        Public Sub AddAttachment(ticketId As Integer, userId As Integer, canViewAll As Boolean, fileName As String, contentType As String, content As Byte())
            If content Is Nothing OrElse content.Length = 0 OrElse content.Length > 5242880 Then Throw New InvalidOperationException("Choose a file between 1 byte and 5 MB.")
            Const sql = "IF NOT EXISTS(SELECT 1 FROM dbo.Tickets WHERE TicketId=@TicketId AND (@CanViewAll=1 OR RequestedByUserId=@UserId)) THROW 51000,'Ticket not found.',1; INSERT dbo.TicketAttachments(TicketId,UploadedByUserId,FileName,ContentType,FileSizeBytes,FileContent) VALUES(@TicketId,@UserId,@FileName,@ContentType,@Size,@Content);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId : command.Parameters.Add("@CanViewAll", SqlDbType.Bit).Value = canViewAll
                command.Parameters.Add("@FileName", SqlDbType.NVarChar, 255).Value = fileName : command.Parameters.Add("@ContentType", SqlDbType.NVarChar, 120).Value = contentType
                command.Parameters.Add("@Size", SqlDbType.Int).Value = content.Length : command.Parameters.Add("@Content", SqlDbType.VarBinary, -1).Value = content
                connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub AssignTicket(ticketId As Integer, technicianUserId As Integer)
            Const sql = "UPDATE dbo.Tickets SET AssignedToUserId=@UserId,Status=CASE WHEN Status=N'Open' THEN N'Assigned' ELSE Status END,UpdatedAtUtc=SYSUTCDATETIME() WHERE TicketId=@TicketId; IF @@ROWCOUNT=0 THROW 51000,'Ticket not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@UserId", SqlDbType.Int).Value = technicianUserId : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub ChangeStatus(ticketId As Integer, status As String)
            Dim allowed = New String() {"Open", "Assigned", "InProgress", "Waiting", "Resolved", "Closed", "Cancelled"}
            If Not allowed.Contains(status) Then Throw New InvalidOperationException("Invalid ticket status.")
            Const sql = "UPDATE dbo.Tickets SET Status=@Status,UpdatedAtUtc=SYSUTCDATETIME(),ResolvedAtUtc=CASE WHEN @Status=N'Resolved' THEN COALESCE(ResolvedAtUtc,SYSUTCDATETIME()) WHEN @Status IN(N'Open',N'Assigned',N'InProgress',N'Waiting') THEN NULL ELSE ResolvedAtUtc END,ClosedAtUtc=CASE WHEN @Status=N'Closed' THEN SYSUTCDATETIME() ELSE ClosedAtUtc END WHERE TicketId=@TicketId; IF @@ROWCOUNT=0 THROW 51000,'Ticket not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub AddComment(ticketId As Integer, authorUserId As Integer, body As String, isInternal As Boolean)
            If String.IsNullOrWhiteSpace(body) Then Throw New InvalidOperationException("Write a comment before posting.")
            Const sql = "INSERT dbo.TicketComments(TicketId,AuthorUserId,Body,IsInternal) VALUES(@TicketId,@UserId,@Body,@Internal);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId : command.Parameters.Add("@UserId", SqlDbType.Int).Value = authorUserId : command.Parameters.Add("@Body", SqlDbType.NVarChar, -1).Value = body.Trim() : command.Parameters.Add("@Internal", SqlDbType.Bit).Value = isInternal : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Function ReadOptions(sql As String, idColumn As String, labelColumn As String) As List(Of LookupOption)
            Dim results As New List(Of LookupOption)()
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New LookupOption With {.Id = reader.GetInt32(reader.GetOrdinal(idColumn)), .Label = reader.GetString(reader.GetOrdinal(labelColumn))})
                    End While
                End Using
            End Using
            Return results
        End Function

        Private Shared Function ReadNullableString(reader As SqlDataReader, columnName As String) As String
            Dim ordinal = reader.GetOrdinal(columnName)
            Return If(reader.IsDBNull(ordinal), String.Empty, reader.GetString(ordinal))
        End Function
    End Class
End Namespace
