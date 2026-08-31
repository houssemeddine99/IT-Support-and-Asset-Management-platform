Public Partial Class SiteMaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        UserInitials.Text = Server.HtmlEncode(Convert.ToString(Session("Initials")))
        UserDisplayName.Text = Server.HtmlEncode(Convert.ToString(Session("DisplayName")))
        UserRole.Text = Server.HtmlEncode(Convert.ToString(Session("RoleName")))
    End Sub
End Class
