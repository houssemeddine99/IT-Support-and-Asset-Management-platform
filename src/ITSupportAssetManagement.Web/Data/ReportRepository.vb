Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models
Namespace Data
    Public NotInheritable Class ReportRepository
        Public Function GetOperationsReport() As OperationsReport
            Const sql = "SELECT (SELECT COUNT(*) FROM dbo.Tickets),(SELECT COUNT(*) FROM dbo.Tickets WHERE Status IN(N'Resolved',N'Closed')),(SELECT COUNT(*) FROM dbo.Tickets WHERE Status NOT IN(N'Resolved',N'Closed',N'Cancelled')),(SELECT COUNT(*) FROM dbo.Assets),(SELECT COUNT(*) FROM dbo.MaintenanceInterventions WHERE Status IN(N'Planned',N'InProgress')),COALESCE((SELECT SUM(COALESCE(LaborCost,0)) FROM dbo.MaintenanceInterventions WHERE Status=N'Completed'),0);" &
                "SELECT Status,COUNT(*) FROM dbo.Tickets GROUP BY Status ORDER BY COUNT(*) DESC;" &
                "SELECT Status,COUNT(*) FROM dbo.Assets GROUP BY Status ORDER BY COUNT(*) DESC;" &
                "SELECT InterventionType,COUNT(*) FROM dbo.MaintenanceInterventions GROUP BY InterventionType ORDER BY COUNT(*) DESC;" &
                "SELECT u.FirstName+N' '+u.LastName,COUNT(DISTINCT CASE WHEN t.Status NOT IN(N'Resolved',N'Closed',N'Cancelled') THEN t.TicketId END),COUNT(DISTINCT CASE WHEN m.Status IN(N'Planned',N'InProgress') THEN m.MaintenanceInterventionId END) FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId LEFT JOIN dbo.Tickets t ON t.AssignedToUserId=u.UserId LEFT JOIN dbo.MaintenanceInterventions m ON m.TechnicianUserId=u.UserId WHERE u.IsActive=1 AND r.Name=N'Technician' GROUP BY u.UserId,u.FirstName,u.LastName ORDER BY 2 DESC,3 DESC;" &
                ";WITH MonthOffsets AS(SELECT 5 N UNION ALL SELECT 4 UNION ALL SELECT 3 UNION ALL SELECT 2 UNION ALL SELECT 1 UNION ALL SELECT 0), Months AS(SELECT DATEADD(month,DATEDIFF(month,0,SYSUTCDATETIME())-N,0) MonthStart FROM MonthOffsets) SELECT LEFT(DATENAME(month,MonthStart),3)+N' '+RIGHT(CONVERT(nvarchar(4),YEAR(MonthStart)),2),(SELECT COUNT(*) FROM dbo.Tickets t WHERE t.CreatedAtUtc>=MonthStart AND t.CreatedAtUtc<DATEADD(month,1,MonthStart)),(SELECT COUNT(*) FROM dbo.Tickets t WHERE t.ResolvedAtUtc>=MonthStart AND t.ResolvedAtUtc<DATEADD(month,1,MonthStart)) FROM Months ORDER BY MonthStart;"
            Dim report As New OperationsReport()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    If reader.Read() Then report.TotalTickets = reader.GetInt32(0) : report.ResolvedTickets = reader.GetInt32(1) : report.OpenTickets = reader.GetInt32(2) : report.TotalAssets = reader.GetInt32(3) : report.ActiveMaintenance = reader.GetInt32(4) : report.TotalMaintenanceCost = reader.GetDecimal(5)
                    If reader.NextResult() Then ReadMetrics(reader, report.TicketsByStatus, report.TotalTickets)
                    If reader.NextResult() Then ReadMetrics(reader, report.AssetsByStatus, report.TotalAssets)
                    If reader.NextResult() Then
                        Dim rows As New List(Of ReportMetric)(), total As Integer = 0
                        While reader.Read() : Dim row As New ReportMetric With {.Label = reader.GetString(0), .Value = reader.GetInt32(1)} : rows.Add(row) : total += row.Value : End While
                        For Each row In rows : row.Percentage = Percentage(row.Value, total) : report.MaintenanceByType.Add(row) : Next
                    End If
                    If reader.NextResult() Then
                        Dim workload As New List(Of TechnicianWorkloadItem)(), maximum As Integer = 0
                        While reader.Read()
                            Dim row As New TechnicianWorkloadItem With {.DisplayName=reader.GetString(0),.OpenTickets=reader.GetInt32(1),.ActiveMaintenance=reader.GetInt32(2)}
                            workload.Add(row) : maximum = Math.Max(maximum, row.TotalWork)
                        End While
                        For Each row In workload : row.LoadPercentage = Percentage(row.TotalWork, maximum) : report.TechnicianWorkload.Add(row) : Next
                    End If
                    If reader.NextResult() Then
                        Dim trend As New List(Of ReportTrendPoint)(), maximum As Integer = 0
                        While reader.Read()
                            Dim row As New ReportTrendPoint With {.MonthLabel=reader.GetString(0),.CreatedCount=reader.GetInt32(1),.ResolvedCount=reader.GetInt32(2)}
                            trend.Add(row) : maximum = Math.Max(maximum, Math.Max(row.CreatedCount,row.ResolvedCount))
                        End While
                        For Each row In trend
                            row.CreatedHeight = If(maximum=0,0D,Math.Max(4D,row.CreatedCount*100D/maximum))
                            row.ResolvedHeight = If(maximum=0,0D,Math.Max(4D,row.ResolvedCount*100D/maximum))
                            report.TicketTrend.Add(row)
                        Next
                    End If
                End Using
            End Using
            Return report
        End Function
        Private Shared Sub ReadMetrics(reader As SqlDataReader, target As List(Of ReportMetric), total As Integer)
            While reader.Read() : target.Add(New ReportMetric With {.Label = reader.GetString(0).Replace("InProgress", "In progress").Replace("InMaintenance", "In maintenance"), .Value = reader.GetInt32(1), .Percentage = Percentage(reader.GetInt32(1), total)}) : End While
        End Sub
        Private Shared Function Percentage(value As Integer, total As Integer) As Decimal
            Return If(total = 0, 0D, Math.Round(value * 100D / total, 1))
        End Function
    End Class
End Namespace
