Imports System.Data.SqlClient
Public Partial Class SiteMaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
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
    End Sub
End Class
