Imports System.Data.SqlClient
Public Partial Class SiteMaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        GlobalSearchPanel.Attributes("data-search-url") = ResolveUrl("~/Search/Index.aspx")
        UserInitials.Text = Server.HtmlEncode(Convert.ToString(Session("Initials")))
        UserDisplayName.Text = Server.HtmlEncode(Convert.ToString(Session("DisplayName")))
        UserRole.Text = Server.HtmlEncode(Convert.ToString(Session("RoleName")))
        Dim userId As Integer
        If Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then
            Try
                Dim role = Convert.ToString(Session("RoleName")), count = New Data.TicketRepository().GetSlaAlertCount(userId, role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician")
                NotificationCount.Text = If(count > 99, "99+", count.ToString()) : NotificationCount.Visible = count > 0
            Catch ex As SqlException
                NotificationCount.Visible = False
            End Try
        End If
        If Not IsPostBack Then GlobalSearchInput.Text = Convert.ToString(Request.QueryString("q"))
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
End Class
