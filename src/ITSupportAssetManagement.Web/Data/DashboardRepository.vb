Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class DashboardRepository
        Public Function GetNavigationSummary(viewerUserId As Integer, canViewAllTickets As Boolean) As NavigationSummary
            Const sql As String = "SELECT (SELECT COUNT(*) FROM dbo.Tickets WHERE Status NOT IN (N'Resolved',N'Closed',N'Cancelled') AND (@CanViewAll=1 OR RequestedByUserId=@ViewerUserId OR AssignedToUserId=@ViewerUserId)),(SELECT COUNT(*) FROM dbo.Assets);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@ViewerUserId", System.Data.SqlDbType.Int).Value = viewerUserId
                command.Parameters.Add("@CanViewAll", System.Data.SqlDbType.Bit).Value = canViewAllTickets
                connection.Open()
                Using reader = command.ExecuteReader()
                    If reader.Read() Then Return New NavigationSummary With {.OpenTickets = reader.GetInt32(0), .TotalAssets = reader.GetInt32(1)}
                End Using
            End Using
            Return New NavigationSummary()
        End Function

        Public Function GetSnapshot() As DashboardSnapshot
            Const sql = "SELECT " &
                "(SELECT COUNT(*) FROM dbo.Tickets WHERE Status NOT IN (N'Resolved',N'Closed',N'Cancelled')) AS OpenTickets," &
                "(SELECT COUNT(*) FROM dbo.Tickets WHERE Status NOT IN (N'Resolved',N'Closed',N'Cancelled') AND Priority IN (N'Critical',N'High')) AS AttentionTickets," &
                "(SELECT COUNT(*) FROM dbo.Assets) AS TotalAssets," &
                "(SELECT COUNT(*) FROM dbo.Assets WHERE Status=N'Assigned') AS AssignedAssets," &
                "(SELECT COUNT(*) FROM dbo.Assets WHERE Status=N'InMaintenance') AS AssetsInMaintenance," &
                "(SELECT COUNT(*) FROM dbo.MaintenanceInterventions WHERE Status IN (N'Planned',N'InProgress') AND ScheduledAtUtc<SYSUTCDATETIME()) AS OverdueMaintenance," &
                "COALESCE((SELECT AVG(CONVERT(decimal(18,2),DATEDIFF(MINUTE,CreatedAtUtc,ResolvedAtUtc))/60) FROM dbo.Tickets WHERE ResolvedAtUtc IS NOT NULL),0) AS AverageResolutionHours," &
                "(SELECT COUNT(*) FROM dbo.Assets WHERE Status IN (N'Available',N'Assigned')) AS HealthyAssets," &
                "(SELECT COUNT(*) FROM dbo.Assets WHERE Status IN (N'Retired',N'Lost')) AS NeedsAttentionAssets;" &
                "SELECT TOP (4) t.TicketId,t.TicketNumber,t.Title,t.Priority,c.Name AS CategoryName,CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName+N' '+u.LastName END AS AssignedToName,t.CreatedAtUtc FROM dbo.Tickets t INNER JOIN dbo.TicketCategories c ON c.TicketCategoryId=t.TicketCategoryId LEFT JOIN dbo.Users u ON u.UserId=t.AssignedToUserId WHERE t.Status NOT IN (N'Resolved',N'Closed',N'Cancelled') ORDER BY CASE t.Priority WHEN N'Critical' THEN 1 WHEN N'High' THEN 2 WHEN N'Medium' THEN 3 ELSE 4 END,t.CreatedAtUtc DESC;" &
                "SELECT TOP (5) ActivityType,Title,Detail,EventAtUtc FROM (" &
                "SELECT N'Ticket' ActivityType,N'Ticket '+TicketNumber+N' created' Title,Title Detail,CreatedAtUtc EventAtUtc FROM dbo.Tickets " &
                "UNION ALL SELECT N'Asset',N'Asset '+AssetTag+N' registered',LTRIM(RTRIM(COALESCE(Manufacturer+N' ',N'')+Model)),CreatedAtUtc FROM dbo.Assets " &
                "UNION ALL SELECT N'Maintenance',N'Maintenance '+LOWER(InterventionType)+N' planned',a.AssetTag+N' - '+a.Model,m.CreatedAtUtc FROM dbo.MaintenanceInterventions m INNER JOIN dbo.Assets a ON a.AssetId=m.AssetId) activity ORDER BY EventAtUtc DESC;"
            Dim snapshot As New DashboardSnapshot()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    If reader.Read() Then
                        snapshot.OpenTickets = reader.GetInt32(0) : snapshot.AttentionTickets = reader.GetInt32(1) : snapshot.TotalAssets = reader.GetInt32(2)
                        snapshot.AssignedAssets = reader.GetInt32(3) : snapshot.AssetsInMaintenance = reader.GetInt32(4) : snapshot.OverdueMaintenance = reader.GetInt32(5)
                        snapshot.AverageResolutionHours = reader.GetDecimal(6) : snapshot.HealthyAssets = reader.GetInt32(7) : snapshot.NeedsAttentionAssets = reader.GetInt32(8)
                    End If
                    If reader.NextResult() Then
                        While reader.Read()
                            snapshot.PriorityTickets.Add(New DashboardPriorityTicket With {
                                .TicketId = reader.GetInt32(0), .TicketNumber = reader.GetString(1), .Title = reader.GetString(2), .Priority = reader.GetString(3),
                                .CategoryName = reader.GetString(4), .AssignedToName = If(reader.IsDBNull(5), String.Empty, reader.GetString(5)), .CreatedAtUtc = reader.GetDateTime(6)})
                        End While
                    End If
                    If reader.NextResult() Then
                        While reader.Read()
                            snapshot.RecentActivities.Add(New DashboardActivityItem With {
                                .ActivityType = reader.GetString(0), .Title = reader.GetString(1), .Detail = reader.GetString(2), .EventAtUtc = reader.GetDateTime(3)})
                        End While
                    End If
                End Using
            End Using
            Return snapshot
        End Function
    End Class
End Namespace
