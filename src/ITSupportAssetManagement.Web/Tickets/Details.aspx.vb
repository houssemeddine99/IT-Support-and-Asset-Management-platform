Imports System.Data.SqlClient
Imports System.Linq
Imports ITSupportAssetManagement.Web.Data

Public Partial Class TicketDetailsPage
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Dim ticketId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), ticketId) Then ShowNotFound() : Return
        Try
            Dim userId As Integer
            If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then ShowNotFound() : Return
            Dim roleName = Convert.ToString(Session("RoleName"))
            Dim canViewAll = roleName = "Administrator" OrElse roleName = "ITManager" OrElse roleName = "Technician"
            Dim ticket = New TicketRepository().GetTicketById(ticketId, userId, canViewAll)
            If ticket Is Nothing Then ShowNotFound() : Return
            TicketTitle.Text = Server.HtmlEncode(ticket.Title)
            TicketNumber.Text = Server.HtmlEncode(ticket.TicketNumber)
            CreatedDate.Text = ticket.CreatedAtUtc.ToLocalTime().ToString("dd MMMM yyyy 'at' HH:mm")
            DescriptionText.Text = Server.HtmlEncode(ticket.Description).Replace(Environment.NewLine, "<br />")
            PriorityBadge.Text = ticket.Priority
            PriorityBadge.CssClass = "status status-" & ticket.Priority.ToLowerInvariant()
            StatusBadge.Text = ticket.Status.Replace("InProgress", "In progress")
            StatusBadge.CssClass = "ticket-state state-" & ticket.Status.ToLowerInvariant()
            CategoryName.Text = Server.HtmlEncode(ticket.CategoryName)
            AssetLabel.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssetLabel), "No related asset", ticket.AssetLabel))
            AssignedToName.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssignedToName), "Unassigned", ticket.AssignedToName))
            RequesterName.Text = Server.HtmlEncode(ticket.RequestedByName)
            RequesterEmail.Text = Server.HtmlEncode(ticket.RequestedByEmail)
            Dim names = ticket.RequestedByName.Split(" "c)
            RequesterInitials.Text = Server.HtmlEncode(String.Join(String.Empty, names.Where(Function(value) value.Length > 0).Take(2).Select(Function(value) value.Substring(0, 1))).ToUpperInvariant())
            DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True
        DetailsPanel.Visible = False
    End Sub
End Class
