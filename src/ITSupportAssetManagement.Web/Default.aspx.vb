Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Models

Public Partial Class HomePage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        BindIdentity()
        Try
            BindSnapshot(New DashboardRepository().GetSnapshot())
        Catch ex As SqlException
            BindSnapshot(New DashboardSnapshot())
        End Try
    End Sub

    Private Sub BindIdentity()
        Dim displayName = Convert.ToString(Session("DisplayName")), firstName = If(String.IsNullOrWhiteSpace(displayName), "team", displayName.Split(" "c)(0))
        Dim greeting = If(DateTime.Now.Hour < 12, "Good morning", If(DateTime.Now.Hour < 18, "Good afternoon", "Good evening"))
        GreetingText.Text = Server.HtmlEncode(greeting & ", " & firstName)
        CurrentDateText.Text = Server.HtmlEncode(DateTime.Now.ToString("dddd, dd MMMM"))
        ShiftText.Text = If(DateTime.Now.Hour < 14, "Morning shift", If(DateTime.Now.Hour < 22, "Afternoon shift", "Night shift"))
    End Sub

    Private Sub BindSnapshot(snapshot As DashboardSnapshot)
        OpenTicketsText.Text = snapshot.OpenTickets.ToString() : AttentionTicketsText.Text = snapshot.AttentionTickets.ToString()
        TotalAssetsText.Text = snapshot.TotalAssets.ToString() : AssignedAssetsText.Text = snapshot.AssignedAssets.ToString()
        MaintenanceAssetsText.Text = snapshot.AssetsInMaintenance.ToString() : OverdueMaintenanceText.Text = snapshot.OverdueMaintenance.ToString()
        AverageResolutionText.Text = snapshot.AverageResolutionHours.ToString("0.0")
        HealthyPercentageText.Text = snapshot.HealthyPercentage.ToString("0.#") : HealthyAssetsText.Text = snapshot.HealthyAssets.ToString() : HealthyLegendPercentage.Text = snapshot.HealthyPercentage.ToString("0.#")
        AttentionAssetsText.Text = snapshot.NeedsAttentionAssets.ToString() : AttentionLegendPercentage.Text = snapshot.AttentionPercentage.ToString("0.#")
        MaintenanceLegendCount.Text = snapshot.AssetsInMaintenance.ToString() : MaintenanceLegendPercentage.Text = snapshot.MaintenancePercentage.ToString("0.#")
        Dim healthyEnd = snapshot.HealthyPercentage, attentionEnd = healthyEnd + snapshot.AttentionPercentage
        If snapshot.TotalAssets = 0 Then
            HealthRing.Attributes("style") = "background:conic-gradient(#25375f 0 100%)"
        Else
            HealthRing.Attributes("style") = String.Format(Globalization.CultureInfo.InvariantCulture, "background:conic-gradient(var(--green) 0 {0}%,var(--orange) {0}% {1}%,var(--red) {1}% 100%)", healthyEnd, attentionEnd)
        End If
        PriorityTicketRepeater.DataSource = snapshot.PriorityTickets : PriorityTicketRepeater.DataBind() : NoPriorityTicketsPanel.Visible = snapshot.PriorityTickets.Count = 0
        ActivityRepeater.DataSource = snapshot.RecentActivities : ActivityRepeater.DataBind() : NoActivityPanel.Visible = snapshot.RecentActivities.Count = 0
    End Sub

    Protected Function FormatAge(value As Object) As String
        Dim created = DirectCast(value, DateTime).ToLocalTime(), age = DateTime.Now - created
        If age.TotalMinutes < 60 Then Return "Reported " & Math.Max(1, CInt(Math.Floor(age.TotalMinutes))).ToString() & " min ago"
        If age.TotalHours < 24 Then Return "Reported " & CInt(Math.Floor(age.TotalHours)).ToString() & "h ago"
        Return "Reported " & CInt(Math.Floor(age.TotalDays)).ToString() & "d ago"
    End Function

    Protected Function DisplayAssignee(value As Object) As String
        Dim name = Convert.ToString(value) : Return If(String.IsNullOrWhiteSpace(name), "Unassigned", name)
    End Function

    Protected Sub ExportButton_Click(sender As Object, e As EventArgs) Handles ExportButton.Click
        Try
            Dim snapshot As DashboardSnapshot = New DashboardRepository().GetSnapshot()
            Dim csv As New StringBuilder()
            csv.AppendLine("Siliana IT Hub Dashboard Report")
            csv.AppendLine("Generated," & DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
            csv.AppendLine()
            csv.AppendLine("Indicator,Value")
            csv.AppendLine("Open tickets," & snapshot.OpenTickets.ToString())
            csv.AppendLine("High-priority attention," & snapshot.AttentionTickets.ToString())
            csv.AppendLine("Total assets," & snapshot.TotalAssets.ToString())
            csv.AppendLine("Assigned assets," & snapshot.AssignedAssets.ToString())
            csv.AppendLine("Assets in maintenance," & snapshot.AssetsInMaintenance.ToString())
            csv.AppendLine("Overdue interventions," & snapshot.OverdueMaintenance.ToString())
            csv.AppendLine("Average resolution hours," & snapshot.AverageResolutionHours.ToString(Globalization.CultureInfo.InvariantCulture))
            csv.AppendLine("Healthy assets," & snapshot.HealthyAssets.ToString())
            csv.AppendLine("Assets needing attention," & snapshot.NeedsAttentionAssets.ToString())
            csv.AppendLine()
            csv.AppendLine("Priority tickets")
            csv.AppendLine("Ticket,Title,Priority,Category,Assignee,Created")
            For Each ticket As DashboardPriorityTicket In snapshot.PriorityTickets
                csv.AppendLine(String.Join(",", EscapeCsv(ticket.TicketNumber), EscapeCsv(ticket.Title), EscapeCsv(ticket.Priority), EscapeCsv(ticket.CategoryName), EscapeCsv(DisplayAssignee(ticket.AssignedToName)), EscapeCsv(ticket.CreatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"))))
            Next
            csv.AppendLine()
            csv.AppendLine("Recent activity")
            csv.AppendLine("Type,Event,Detail,Date")
            For Each activity As DashboardActivityItem In snapshot.RecentActivities
                csv.AppendLine(String.Join(",", EscapeCsv(activity.ActivityType), EscapeCsv(activity.Title), EscapeCsv(activity.Detail), EscapeCsv(activity.EventAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"))))
            Next

            Response.Clear()
            Response.ContentType = "text/csv"
            Response.ContentEncoding = Encoding.UTF8
            Response.AddHeader("Content-Disposition", "attachment; filename=Siliana-Dashboard-" & DateTime.Now.ToString("yyyyMMdd-HHmm") & ".csv")
            Response.Write(ChrW(&HFEFF) & csv.ToString())
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As SqlException
            ExportButton.Enabled = False
            ExportButton.Text = "Export unavailable"
        End Try
    End Sub

    Private Shared Function EscapeCsv(value As String) As String
        Dim safeValue As String = If(value, String.Empty)
        Return """" & safeValue.Replace("""", """""") & """"
    End Function
End Class
