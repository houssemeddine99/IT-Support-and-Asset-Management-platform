Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models
Namespace Data
    Public NotInheritable Class ReportRepository
        Public Function GetOperationsReport() As OperationsReport
            Const sql = "SELECT (SELECT COUNT(*) FROM dbo.Tickets),(SELECT COUNT(*) FROM dbo.Tickets WHERE Status IN(N'Resolved',N'Closed')),(SELECT COUNT(*) FROM dbo.Tickets WHERE Status NOT IN(N'Resolved',N'Closed',N'Cancelled')),(SELECT COUNT(*) FROM dbo.Assets),(SELECT COUNT(*) FROM dbo.MaintenanceInterventions WHERE Status IN(N'Planned',N'InProgress')),COALESCE((SELECT SUM(COALESCE(LaborCost,0)) FROM dbo.MaintenanceInterventions WHERE Status=N'Completed'),0);" &
                "SELECT Status,COUNT(*) FROM dbo.Tickets GROUP BY Status ORDER BY COUNT(*) DESC;" &
                "SELECT Status,COUNT(*) FROM dbo.Assets GROUP BY Status ORDER BY COUNT(*) DESC;" &
                "SELECT InterventionType,COUNT(*) FROM dbo.MaintenanceInterventions GROUP BY InterventionType ORDER BY COUNT(*) DESC;"
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
