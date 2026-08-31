Imports System.Data.SqlClient
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
        HealthRing.Attributes("style") = String.Format(Globalization.CultureInfo.InvariantCulture, "background:conic-gradient(var(--green) 0 {0}%,var(--orange) {0}% {1}%,var(--red) {1}% 100%)", healthyEnd, attentionEnd)
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
End Class
