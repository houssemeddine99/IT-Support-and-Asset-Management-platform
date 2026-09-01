Imports System.Data.SqlClient
Imports System.Configuration
Imports ITSupportAssetManagement.Web.Security
Public Partial Class SiteMaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim path As String = Request.AppRelativeCurrentExecutionFilePath
        If Convert.ToBoolean(If(Session("MustChangePassword"), False)) AndAlso Not path.Equals("~/Account/Profile.aspx", StringComparison.OrdinalIgnoreCase) Then
            Response.Redirect(ResolveUrl("~/Account/Profile.aspx?required=1"), False)
            Context.ApplicationInstance.CompleteRequest()
            Return
        End If
        Dim currentRole As String = Convert.ToString(Session("RoleName"))
        If Not AuthorizationService.CanAccessPath(currentRole, path) Then
            Response.Redirect(ResolveUrl("~/AccessDenied.aspx"), False)
            Context.ApplicationInstance.CompleteRequest()
            Return
        End If
        BindRoleNavigation(currentRole)
        GlobalSearchPanel.Attributes("data-search-url") = ResolveUrl("~/Search/Index.aspx")
        UserInitials.Text = Server.HtmlEncode(Convert.ToString(Session("Initials")))
        UserDisplayName.Text = Server.HtmlEncode(Convert.ToString(Session("DisplayName")))
        UserRole.Text = Server.HtmlEncode(Convert.ToString(Session("RoleName")))
        Dim userId As Integer
        If Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then
            Try
                Dim role = Convert.ToString(Session("RoleName")), canViewAll = role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician"
                Dim count = New Data.TicketRepository().GetSlaAlertCount(userId, canViewAll)
                NotificationCount.Text = If(count > 99, "99+", count.ToString()) : NotificationCount.Visible = count > 0
                BindNavigationSummary(New Data.DashboardRepository().GetNavigationSummary(userId, canViewAll))
            Catch ex As SqlException
                NotificationCount.Visible = False
                OpenTicketCount.Visible = False
                BindAssetCapacity(0)
            End Try
        End If
        If Not IsPostBack Then GlobalSearchInput.Text = Convert.ToString(Request.QueryString("q"))
    End Sub

    Private Sub BindRoleNavigation(roleName As String)
        Dim isAdministrator As Boolean = roleName = "Administrator"
        Dim isManager As Boolean = roleName = "ITManager"
        TeamNavigation.Visible = isAdministrator OrElse isManager
        ReportsNavigation.Visible = isAdministrator OrElse isManager
        AuditNavigation.Visible = isAdministrator
        SettingsNavigation.Visible = isAdministrator
    End Sub

    Protected Sub GlobalSearchButton_Click(sender As Object, e As EventArgs) Handles GlobalSearchButton.Click
        Dim query As String = GlobalSearchInput.Text.Trim()
        If query.Length = 0 Then Return
        Response.Redirect(ResolveUrl("~/Search/Index.aspx?q=" & Server.UrlEncode(query)), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Protected Function NavClass(section As String) As String
        Dim path As String = Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant()
        Dim active As Boolean = If(section = "overview", path = "~/default.aspx", path.StartsWith("~/" & section & "/", StringComparison.Ordinal))
        Return If(active, "nav-link active", "nav-link")
    End Function

    Private Sub BindNavigationSummary(summary As Models.NavigationSummary)
        OpenTicketCount.InnerText = If(summary.OpenTickets > 99, "99+", summary.OpenTickets.ToString())
        OpenTicketCount.Visible = summary.OpenTickets > 0
        BindAssetCapacity(summary.TotalAssets)
    End Sub

    Private Sub BindAssetCapacity(totalAssets As Integer)
        Dim capacity As Integer
        If Not Integer.TryParse(ConfigurationManager.AppSettings("AssetCapacity"), capacity) OrElse capacity < 1 Then capacity = 400
        Dim percentage As Decimal = Math.Round(totalAssets * 100D / capacity, 0)
        AssetCapacityPercentage.Text = percentage.ToString("0") & "%"
        AssetCapacityDetail.Text = totalAssets.ToString() & " of " & capacity.ToString() & " assets registered"
        AssetCapacityBar.Style("width") = Math.Min(percentage, 100D).ToString("0", Globalization.CultureInfo.InvariantCulture) & "%"
        AssetCapacityBar.Attributes("aria-label") = AssetCapacityDetail.Text
    End Sub
End Class
