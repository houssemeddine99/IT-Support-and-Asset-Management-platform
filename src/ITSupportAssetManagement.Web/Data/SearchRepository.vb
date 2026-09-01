Imports System.Data
Imports System.Data.SqlClient

Namespace Data
    Public Class SearchRepository
        Public Function Search(query As String, userId As Integer, canSeeAllTickets As Boolean) As List(Of Models.GlobalSearchResult)
            Const sql As String =
                "SELECT TOP (30) EntityType, EntityId, Title, Subtitle, Status, UpdatedAtUtc " &
                "FROM (" &
                " SELECT N'Ticket' EntityType, t.TicketId EntityId, t.Title, CONCAT(t.TicketNumber, N' · ', c.Name, N' · ', u.FirstName, N' ', u.LastName) Subtitle, t.Status, COALESCE(t.UpdatedAtUtc,t.CreatedAtUtc) UpdatedAtUtc" &
                " FROM dbo.Tickets t INNER JOIN dbo.TicketCategories c ON c.TicketCategoryId=t.TicketCategoryId INNER JOIN dbo.Users u ON u.UserId=t.RequestedByUserId" &
                " WHERE (@CanSeeAll=1 OR t.RequestedByUserId=@UserId OR t.AssignedToUserId=@UserId) AND (t.TicketNumber LIKE @Pattern OR t.Title LIKE @Pattern OR t.Description LIKE @Pattern)" &
                " UNION ALL" &
                " SELECT N'Asset', a.AssetId, CONCAT(a.AssetTag, N' · ', COALESCE(NULLIF(a.Manufacturer,N'') + N' ',N''), a.Model), CONCAT(c.Name, N' · ', COALESCE(NULLIF(a.Location,N''),N'Location not recorded')), a.Status, COALESCE(a.UpdatedAtUtc,a.CreatedAtUtc)" &
                " FROM dbo.Assets a INNER JOIN dbo.AssetCategories c ON c.AssetCategoryId=a.AssetCategoryId" &
                " WHERE a.AssetTag LIKE @Pattern OR a.SerialNumber LIKE @Pattern OR a.Manufacturer LIKE @Pattern OR a.Model LIKE @Pattern OR a.Location LIKE @Pattern" &
                " UNION ALL" &
                " SELECT N'Person', u.UserId, CONCAT(u.FirstName,N' ',u.LastName), CONCAT(COALESCE(NULLIF(u.EmployeeCode,N''),N'No employee code'),N' · ',COALESCE(NULLIF(u.Department,N''),r.Name)), CASE WHEN u.IsActive=1 THEN r.Name ELSE N'Inactive' END, COALESCE(u.UpdatedAtUtc,u.CreatedAtUtc)" &
                " FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId" &
                " WHERE u.FirstName LIKE @Pattern OR u.LastName LIKE @Pattern OR u.Email LIKE @Pattern OR u.EmployeeCode LIKE @Pattern OR u.Department LIKE @Pattern" &
                ") results ORDER BY UpdatedAtUtc DESC;"

            Dim items As New List(Of Models.GlobalSearchResult)()
            Using connection As SqlConnection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Pattern", SqlDbType.NVarChar, 104).Value = "%" & query & "%"
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId
                command.Parameters.Add("@CanSeeAll", SqlDbType.Bit).Value = canSeeAllTickets
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim entityType As String = reader.GetString(0), entityId As Integer = reader.GetInt32(1)
                        items.Add(New Models.GlobalSearchResult With {
                            .EntityType = entityType,
                            .Title = reader.GetString(2),
                            .Subtitle = reader.GetString(3),
                            .Status = reader.GetString(4),
                            .NavigateUrl = BuildUrl(entityType, entityId),
                            .UpdatedAtUtc = reader.GetDateTime(5)
                        })
                    End While
                End Using
            End Using
            Return items
        End Function

        Private Shared Function BuildUrl(entityType As String, entityId As Integer) As String
            Select Case entityType
                Case "Ticket" : Return "~/Tickets/Details.aspx?id=" & entityId.ToString()
                Case "Asset" : Return "~/Assets/Details.aspx?id=" & entityId.ToString()
                Case Else : Return "~/Team/Edit.aspx?id=" & entityId.ToString()
            End Select
        End Function
    End Class
End Namespace
