Imports System.Data.SqlClient
Imports System.IO
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
            SetSlaBadge(ticket.DueAtUtc, ticket.Status)
            CategoryName.Text = Server.HtmlEncode(ticket.CategoryName) : AssetLabel.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssetLabel), "No related asset", ticket.AssetLabel)) : AssignedToName.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(ticket.AssignedToName), "Unassigned", ticket.AssignedToName))
            RequesterName.Text = Server.HtmlEncode(ticket.RequestedByName) : RequesterEmail.Text = Server.HtmlEncode(ticket.RequestedByEmail)
            Dim names = ticket.RequestedByName.Split(" "c) : RequesterInitials.Text = Server.HtmlEncode(String.Join(String.Empty, names.Where(Function(value) value.Length > 0).Take(2).Select(Function(value) value.Substring(0, 1))).ToUpperInvariant())
            WorkflowPanel.Visible = canManage : InternalInput.Visible = canManage
            EditLink.NavigateUrl = "Edit.aspx?id=" & ticketId.ToString() : EditLink.Visible = canManage
            If canManage Then
                TechnicianInput.DataSource = _tickets.GetAssignableTechnicians() : TechnicianInput.DataTextField = "Label" : TechnicianInput.DataValueField = "Id" : TechnicianInput.DataBind() : TechnicianInput.Items.Insert(0, New ListItem("Select technician", String.Empty)) : StatusInput.SelectedValue = ticket.Status
            End If
            Dim comments = _tickets.GetComments(ticketId, canManage) : CommentRepeater.DataSource = comments : CommentRepeater.DataBind() : NoCommentsPanel.Visible = comments.Count = 0
            Dim attachments = _tickets.GetAttachments(ticketId, userId, canManage) : AttachmentRepeater.DataSource = attachments : AttachmentRepeater.DataBind() : NoAttachmentsPanel.Visible = attachments.Count = 0
            If Request.QueryString("updated") = "1" Then SuccessPanel.Visible = True
            DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Protected Sub AssignButton_Click(sender As Object, e As EventArgs) Handles AssignButton.Click
        If Not IsStaff(Convert.ToString(Session("RoleName"))) Then Return
        Dim technicianId As Integer : If Not Integer.TryParse(TechnicianInput.SelectedValue, technicianId) Then ShowError("Select a technician.") : Return
        ExecuteUpdate(Sub() _tickets.AssignTicket(GetTicketId(), technicianId), "Assigned", "Assigned ticket to technician user " & technicianId.ToString())
    End Sub
    Protected Sub StatusButton_Click(sender As Object, e As EventArgs) Handles StatusButton.Click
        If IsStaff(Convert.ToString(Session("RoleName"))) Then ExecuteUpdate(Sub() _tickets.ChangeStatus(GetTicketId(), StatusInput.SelectedValue), "Status changed", "Changed ticket status to " & StatusInput.SelectedValue)
    End Sub
    Protected Sub CommentButton_Click(sender As Object, e As EventArgs) Handles CommentButton.Click
        Dim userId As Integer : If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Return
        Try
            _tickets.AddComment(GetTicketId(), userId, CommentInput.Text, InternalInput.Checked AndAlso IsStaff(Convert.ToString(Session("RoleName")))) : AuditRepository.Record("Commented", "Ticket", GetTicketId().ToString(), "Added a ticket comment") : Response.Redirect("~/Tickets/Details.aspx?id=" & GetTicketId().ToString() & "&updated=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The comment could not be saved.")
        End Try
    End Sub
    Protected Sub UploadButton_Click(sender As Object, e As EventArgs) Handles UploadButton.Click
        Dim userId As Integer : If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Return
        If Not AttachmentInput.HasFile Then ShowError("Choose a file to upload.") : Return
        Dim safeName = Path.GetFileName(AttachmentInput.FileName), extension = Path.GetExtension(safeName).ToLowerInvariant()
        Dim allowed = New String() {".png", ".jpg", ".jpeg", ".pdf", ".txt", ".log", ".docx", ".xlsx"}
        If String.IsNullOrWhiteSpace(safeName) OrElse Not allowed.Contains(extension) Then ShowError("Allowed files: PNG, JPG, PDF, TXT, LOG, DOCX, and XLSX.") : Return
        If AttachmentInput.PostedFile.ContentLength <= 0 OrElse AttachmentInput.PostedFile.ContentLength > 5242880 Then ShowError("The file must be no larger than 5 MB.") : Return
        Dim role = Convert.ToString(Session("RoleName")), canManage = IsStaff(role), contentType = AttachmentInput.PostedFile.ContentType
        If String.IsNullOrWhiteSpace(contentType) OrElse contentType.Length > 120 Then contentType = "application/octet-stream"
        Try
            _tickets.AddAttachment(GetTicketId(), userId, canManage, safeName, contentType, AttachmentInput.FileBytes)
            AuditRepository.Record("Attachment uploaded", "Ticket", GetTicketId().ToString(), "Uploaded " & safeName)
            Response.Redirect("~/Tickets/Details.aspx?id=" & GetTicketId().ToString() & "&updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException
            ShowError("The attachment could not be uploaded.")
        End Try
    End Sub
    Private Sub ExecuteUpdate(action As Action, auditAction As String, summary As String)
        Try
            action() : AuditRepository.Record(auditAction, "Ticket", GetTicketId().ToString(), summary) : Response.Redirect("~/Tickets/Details.aspx?id=" & GetTicketId().ToString() & "&updated=1", False)
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
    Protected Shared Function GetAttachmentIcon(value As Object) As String
        Dim extension = Path.GetExtension(Convert.ToString(value)).ToLowerInvariant()
        If extension = ".png" OrElse extension = ".jpg" OrElse extension = ".jpeg" Then Return "bi bi-file-earmark-image"
        If extension = ".pdf" Then Return "bi bi-file-earmark-pdf"
        If extension = ".docx" Then Return "bi bi-file-earmark-word"
        If extension = ".xlsx" Then Return "bi bi-file-earmark-excel"
        Return "bi bi-file-earmark-text"
    End Function
    Private Sub SetSlaBadge(dueAtUtc As DateTime?, status As String)
        If Not dueAtUtc.HasValue OrElse status = "Resolved" OrElse status = "Closed" OrElse status = "Cancelled" Then SlaBadge.Text = "Not active" : SlaBadge.CssClass = "sla-chip neutral" : Return
        Dim remaining = dueAtUtc.Value - DateTime.UtcNow
        If remaining.TotalMinutes < 0 Then SlaBadge.Text = "Overdue · " & dueAtUtc.Value.ToLocalTime().ToString("dd MMM, HH:mm") : SlaBadge.CssClass = "sla-chip overdue" : Return
        SlaBadge.Text = "Due " & dueAtUtc.Value.ToLocalTime().ToString("dd MMM, HH:mm") : SlaBadge.CssClass = If(remaining.TotalHours <= 4, "sla-chip warning", "sla-chip healthy")
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True : DetailsPanel.Visible = False
    End Sub
End Class
