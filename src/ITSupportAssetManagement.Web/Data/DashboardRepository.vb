Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class DashboardRepository
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
                "SELECT TOP (4) t.TicketId,t.TicketNumber,t.Title,t.Priority,c.Name AS CategoryName,CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName+N' '+u.LastName END AS AssignedToName,t.CreatedAtUtc FROM dbo.Tickets t INNER JOIN dbo.TicketCategories c ON c.TicketCategoryId=t.TicketCategoryId LEFT JOIN dbo.Users u ON u.UserId=t.AssignedToUserId WHERE t.Status NOT IN (N'Resolved',N'Closed',N'Cancelled') ORDER BY CASE t.Priority WHEN N'Critical' THEN 1 WHEN N'High' THEN 2 WHEN N'Medium' THEN 3 ELSE 4 END,t.CreatedAtUtc DESC;"
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
                End Using
            End Using
            Return snapshot
        End Function
    End Class
End Namespace
