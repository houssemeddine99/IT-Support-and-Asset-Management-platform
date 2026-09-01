Imports System.Data.SqlClient
Imports System.Globalization
Imports ITSupportAssetManagement.Web.Data
Public Partial Class MaintenanceEditPage
    Inherits System.Web.UI.Page
    Private ReadOnly _maintenance As New MaintenanceRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role = Convert.ToString(Session("RoleName")) : If role <> "Administrator" AndAlso role <> "ITManager" AndAlso role <> "Technician" Then Response.Redirect("~/Default.aspx", False) : Return
        Dim id = GetId() : BackLink.NavigateUrl = "Details.aspx?id=" & id.ToString() : CancelLink.NavigateUrl = BackLink.NavigateUrl : If Not IsPostBack Then LoadForm(id)
    End Sub
    Private Sub LoadForm(id As Integer)
        Try
            Dim item = _maintenance.GetById(id) : If item Is Nothing OrElse (item.Status <> "Planned" AndAlso item.Status <> "InProgress") Then Response.Redirect("~/Maintenance/Details.aspx?id=" & id.ToString(), False) : Return
            TechnicianInput.DataSource = _maintenance.GetTechnicians() : TechnicianInput.DataTextField = "Label" : TechnicianInput.DataValueField = "Id" : TechnicianInput.DataBind() : TechnicianInput.Items.Insert(0, New ListItem("Unassigned", String.Empty))
            If item.TechnicianUserId.HasValue Then TechnicianInput.SelectedValue = item.TechnicianUserId.Value.ToString()
            TypeInput.SelectedValue = item.InterventionType : If item.ScheduledAtUtc.HasValue Then ScheduledInput.Text = item.ScheduledAtUtc.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm")
            ProviderInput.Text = item.ExternalProvider : DiagnosisInput.Text = item.Diagnosis
        Catch ex As SqlException : ShowError("The intervention could not be loaded.") : End Try
    End Sub
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim technicianValue As Integer, technician As Integer? = Nothing : If Integer.TryParse(TechnicianInput.SelectedValue, technicianValue) Then technician = technicianValue
        Dim scheduled As DateTime? = Nothing, parsed As DateTime
        If Not String.IsNullOrWhiteSpace(ScheduledInput.Text) Then
            If Not DateTime.TryParseExact(ScheduledInput.Text, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed) Then ShowError("Enter a valid schedule.") : Return
            scheduled = DateTime.SpecifyKind(parsed, DateTimeKind.Local).ToUniversalTime()
        End If
        Try : _maintenance.UpdateIntervention(GetId(), technician, TypeInput.SelectedValue, scheduled, DiagnosisInput.Text, ProviderInput.Text) : Response.Redirect("~/Maintenance/Details.aspx?id=" & GetId().ToString() & "&updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException : ShowError("The intervention could not be saved.") : End Try
    End Sub
    Private Function GetId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
