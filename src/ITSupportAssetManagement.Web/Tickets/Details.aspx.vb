Imports System.Data.SqlClient
Imports System.Linq
Imports ITSupportAssetManagement.Web.Data

Public Partial Class TicketDetailsPage
    Inherits System.Web.UI.Page
    Private ReadOnly _tickets As New TicketRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then LoadTicket()
    End Sub
    Private Sub LoadTicket()
        Dim ticketId = GetTicketId(), userId As Integer
        If ticketId = 0 OrElse Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then ShowNotFound() : Return
        Try
            Dim roleName = Convert.ToString(Session("RoleName")), canManage = IsStaff(roleName), ticket = _tickets.GetTicketById(ticketId, userId, canManage)
            If ticket Is Nothing Then ShowNotFound() : Return
            TicketTitle.Text = Server.HtmlEncode(ticket.Title) : TicketNumber.Text = Server.HtmlEncode(ticket.TicketNumber) : CreatedDate.Text = ticket.CreatedAtUtc.ToLocalTime().ToString("dd MMMM yyyy 'at' HH:mm")
            DescriptionText.Text = Server.HtmlEncode(ticket.Description).Replace(Environment.NewLine, "<br />") : PriorityBadge.Text = ticket.Priority : PriorityBadge.CssClass = "status status-" & ticket.Priority.ToLowerInvariant()
            StatusBadge.Text = ticket.Status.Replace("InProgress", "In progress") : StatusBadge.CssClass = "ticket-state state-" & ticket.Status.ToLowerInvariant()
            CategoryName.Text = Server.HtmlEncode(ticket.CategoryName) : AssetLabel.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssetLabel), "No related asset", ticket.AssetLabel)) : AssignedToName.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssignedToName), "Unassigned", ticket.AssignedToName))
            RequesterName.Text = Server.HtmlEncode(ticket.RequestedByName) : RequesterEmail.Text = Server.HtmlEncode(ticket.RequestedByEmail)
            Dim names = ticket.RequestedByName.Split(" "c) : RequesterInitials.Text = Server.HtmlEncode(String.Join(String.Empty, names.Where(Function(value) value.Length > 0).Take(2).Select(Function(value) value.Substring(0, 1))).ToUpperInvariant())
            WorkflowPanel.Visible = canManage : InternalInput.Visible = canManage
            If canManage Then
                TechnicianInput.DataSource = _tickets.GetAssignableTechnicians() : TechnicianInput.DataTextField = "Label" : TechnicianInput.DataValueField = "Id" : TechnicianInput.DataBind() : TechnicianInput.Items.Insert(0, New ListItem("Select technician", String.Empty)) : StatusInput.SelectedValue = ticket.Status
            End If
            Dim comments = _tickets.GetComments(ticketId, canManage) : CommentRepeater.DataSource = comments : CommentRepeater.DataBind() : NoCommentsPanel.Visible = comments.Count = 0
            If Request.QueryString("updated") = "1" Then SuccessPanel.Visible = True
            DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Protected Sub AssignButton_Click(sender As Object, e As EventArgs) Handles AssignButton.Click
        If Not IsStaff(Convert.ToString(Session("RoleName"))) Then Return
        Dim technicianId As Integer : If Not Integer.TryParse(TechnicianInput.SelectedValue, technicianId) Then ShowError("Select a technician.") : Return
        ExecuteUpdate(Sub() _tickets.AssignTicket(GetTicketId(), technicianId))
    End Sub
    Protected Sub StatusButton_Click(sender As Object, e As EventArgs) Handles StatusButton.Click
        If IsStaff(Convert.ToString(Session("RoleName"))) Then ExecuteUpdate(Sub() _tickets.ChangeStatus(GetTicketId(), StatusInput.SelectedValue))
    End Sub
    Protected Sub CommentButton_Click(sender As Object, e As EventArgs) Handles CommentButton.Click
        Dim userId As Integer : If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Return
        Try
            _tickets.AddComment(GetTicketId(), userId, CommentInput.Text, InternalInput.Checked AndAlso IsStaff(Convert.ToString(Session("RoleName")))) : Response.Redirect("~/Tickets/Details.aspx?id=" & GetTicketId().ToString() & "&updated=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The comment could not be saved.")
        End Try
    End Sub
    Private Sub ExecuteUpdate(action As Action)
        Try
            action() : Response.Redirect("~/Tickets/Details.aspx?id=" & GetTicketId().ToString() & "&updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException
            ShowError("The ticket could not be updated. Refresh and try again.")
        End Try
    End Sub
    Private Function GetTicketId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Shared Function IsStaff(roleName As String) As Boolean
        Return roleName = "Administrator" OrElse roleName = "ITManager" OrElse roleName = "Technician"
    End Function
    Protected Function FormatCommentDate(value As Object) As String
        Return DirectCast(value, DateTime).ToLocalTime().ToString("dd MMM yyyy, HH:mm")
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True : DetailsPanel.Visible = False
    End Sub
End Class
