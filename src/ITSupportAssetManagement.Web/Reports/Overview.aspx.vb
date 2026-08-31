Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Models
Public Partial Class ReportsOverviewPage
    Inherits System.Web.UI.Page
    Private ReadOnly _reports As New ReportRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role = Convert.ToString(Session("RoleName"))
        If role <> "Administrator" AndAlso role <> "ITManager" Then Response.Redirect("~/Default.aspx", False) : Return
        If Not IsPostBack Then BindReport()
    End Sub
    Private Sub BindReport()
        Try
            Dim report = _reports.GetOperationsReport() : ViewState("Report") = Nothing
            TotalTicketsText.Text = report.TotalTickets.ToString() : OpenTicketsText.Text = report.OpenTickets.ToString() : ResolvedTicketsText.Text = report.ResolvedTickets.ToString() : ResolutionRateText.Text = report.ResolutionRate.ToString("0.#")
            TotalAssetsText.Text = report.TotalAssets.ToString() : ActiveMaintenanceText.Text = report.ActiveMaintenance.ToString() : MaintenanceCostText.Text = report.TotalMaintenanceCost.ToString("N2")
            TicketMetricsRepeater.DataSource = report.TicketsByStatus : TicketMetricsRepeater.DataBind() : NoTicketData.Visible = report.TicketsByStatus.Count = 0
            AssetMetricsRepeater.DataSource = report.AssetsByStatus : AssetMetricsRepeater.DataBind() : NoAssetData.Visible = report.AssetsByStatus.Count = 0
            MaintenanceMetricsRepeater.DataSource = report.MaintenanceByType : MaintenanceMetricsRepeater.DataBind() : NoMaintenanceData.Visible = report.MaintenanceByType.Count = 0
        Catch ex As SqlException
            ErrorMessage.Text = "The report could not be generated from the database." : ErrorPanel.Visible = True : ExportButton.Enabled = False
        End Try
    End Sub
    Protected Sub ExportButton_Click(sender As Object, e As EventArgs) Handles ExportButton.Click
        Try
            Dim report = _reports.GetOperationsReport(), csv As New StringBuilder()
            csv.AppendLine("Siliana IT Hub Operations Report").AppendLine("Generated," & DateTime.Now.ToString("yyyy-MM-dd HH:mm")).AppendLine()
            csv.AppendLine("Indicator,Value").AppendLine("Total tickets," & report.TotalTickets).AppendLine("Open tickets," & report.OpenTickets).AppendLine("Resolved tickets," & report.ResolvedTickets).AppendLine("Resolution rate," & report.ResolutionRate.ToString(Globalization.CultureInfo.InvariantCulture) & "%").AppendLine("Total assets," & report.TotalAssets).AppendLine("Active maintenance," & report.ActiveMaintenance).AppendLine("Maintenance labor cost," & report.TotalMaintenanceCost.ToString(Globalization.CultureInfo.InvariantCulture)).AppendLine()
            AppendMetrics(csv, "Ticket status", report.TicketsByStatus) : AppendMetrics(csv, "Asset status", report.AssetsByStatus) : AppendMetrics(csv, "Maintenance type", report.MaintenanceByType)
            Response.Clear() : Response.ContentType = "text/csv" : Response.ContentEncoding = Encoding.UTF8 : Response.AddHeader("Content-Disposition", "attachment; filename=Siliana-IT-Report-" & DateTime.Now.ToString("yyyyMMdd") & ".csv") : Response.Write(ChrW(&HFEFF) & csv.ToString()) : Response.Flush() : HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As SqlException
            ErrorMessage.Text = "The CSV report could not be generated." : ErrorPanel.Visible = True
        End Try
    End Sub
    Private Shared Sub AppendMetrics(csv As StringBuilder, title As String, rows As List(Of ReportMetric))
        csv.AppendLine(title & ",Count,Percentage")
        For Each row In rows : csv.AppendLine(EscapeCsv(row.Label) & "," & row.Value & "," & row.Percentage.ToString(Globalization.CultureInfo.InvariantCulture) & "%") : Next
        csv.AppendLine()
    End Sub
    Private Shared Function EscapeCsv(value As String) As String
        Return """" & value.Replace("""", """""") & """"
    End Function
End Class
