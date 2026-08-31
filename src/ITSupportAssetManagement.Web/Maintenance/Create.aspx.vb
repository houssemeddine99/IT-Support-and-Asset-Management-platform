Imports System.Data.SqlClient
Imports System.Globalization
Imports ITSupportAssetManagement.Web.Data
Public Partial Class MaintenanceCreatePage
    Inherits System.Web.UI.Page
    Private ReadOnly _maintenance As New MaintenanceRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleName = Convert.ToString(Session("RoleName"))
        If roleName <> "Administrator" AndAlso roleName <> "ITManager" AndAlso roleName <> "Technician" Then Response.Redirect("~/Maintenance/List.aspx", False) : Return
        If Not IsPostBack Then LoadOptions()
    End Sub
    Private Sub LoadOptions()
        Try
            AssetInput.DataSource = _maintenance.GetEligibleAssets() : AssetInput.DataTextField = "Label" : AssetInput.DataValueField = "Id" : AssetInput.DataBind() : AssetInput.Items.Insert(0, New ListItem("Select an asset", String.Empty))
            If Not String.IsNullOrWhiteSpace(Request.QueryString("assetId")) AndAlso AssetInput.Items.FindByValue(Request.QueryString("assetId")) IsNot Nothing Then AssetInput.SelectedValue = Request.QueryString("assetId")
            TechnicianInput.DataSource = _maintenance.GetTechnicians() : TechnicianInput.DataTextField = "Label" : TechnicianInput.DataValueField = "Id" : TechnicianInput.DataBind() : TechnicianInput.Items.Insert(0, New ListItem("Assign later", String.Empty))
            Dim currentUserId = Convert.ToString(Session("UserId")) : If TechnicianInput.Items.FindByValue(currentUserId) IsNot Nothing Then TechnicianInput.SelectedValue = currentUserId
            ScheduledInput.Text = DateTime.Now.AddHours(1).ToString("yyyy-MM-ddTHH:mm")
        Catch ex As SqlException
            ShowError("Assets and technicians could not be loaded.") : CreateButton.Enabled = False
        End Try
    End Sub
    Protected Sub CreateButton_Click(sender As Object, e As EventArgs) Handles CreateButton.Click
        If Not Page.IsValid Then Return
        Dim assetId As Integer, technicianId As Integer, technician As Integer? = Nothing, scheduled As DateTime? = Nothing, parsedSchedule As DateTime
        If Not Integer.TryParse(AssetInput.SelectedValue, assetId) Then ShowError("Select a valid asset.") : Return
        If Integer.TryParse(TechnicianInput.SelectedValue, technicianId) Then technician = technicianId
        If Not String.IsNullOrWhiteSpace(ScheduledInput.Text) Then
            If Not DateTime.TryParseExact(ScheduledInput.Text, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, parsedSchedule) Then ShowError("The scheduled date is invalid.") : Return
            scheduled = DateTime.SpecifyKind(parsedSchedule, DateTimeKind.Local).ToUniversalTime()
        End If
        Try
            Dim id = _maintenance.CreateIntervention(assetId, technician, TypeInput.SelectedValue, scheduled, DiagnosisInput.Text, ProviderInput.Text)
            Response.Redirect("~/Maintenance/Details.aspx?id=" & id.ToString() & "&created=1", False)
        Catch ex As SqlException
            ShowError("The intervention could not be saved. Verify the information and try again.")
        End Try
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
