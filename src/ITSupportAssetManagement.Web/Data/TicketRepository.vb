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
