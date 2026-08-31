Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetAssignPage
    Inherits System.Web.UI.Page
    Private ReadOnly _assets As New AssetRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleName = Convert.ToString(Session("RoleName"))
        If roleName <> "Administrator" AndAlso roleName <> "ITManager" Then Response.Redirect("~/Assets/List.aspx", False) : Return
        If Not IsPostBack Then LoadForm()
    End Sub
    Protected Sub ConfirmValidator_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles ConfirmValidator.ServerValidate
        args.IsValid = ConfirmInput.Checked
    End Sub
    Protected Sub AssignButton_Click(sender As Object, e As EventArgs) Handles AssignButton.Click
        Page.Validate()
        If Not Page.IsValid Then Return
        Dim assetId As Integer, userId As Integer, assignedBy As Integer
        If Not Integer.TryParse(Request.QueryString("id"), assetId) OrElse Not Integer.TryParse(UserInput.SelectedValue, userId) OrElse Not Integer.TryParse(Convert.ToString(Session("UserId")), assignedBy) Then ShowError("The assignment information is invalid.") : Return
        Try
            _assets.AssignAsset(assetId, userId, assignedBy, NotesInput.Text)
            Response.Redirect("~/Assets/Details.aspx?id=" & assetId.ToString() & "&assigned=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The assignment could not be saved. Refresh the asset and try again.")
        End Try
    End Sub
    Private Sub LoadForm()
        Dim assetId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), assetId) Then Response.Redirect("~/Assets/List.aspx", False) : Return
        Try
            Dim asset = _assets.GetAssetById(assetId)
            If asset Is Nothing OrElse asset.Status <> "Available" Then Response.Redirect("~/Assets/Details.aspx?id=" & assetId.ToString(), False) : Return
            AssetName.Text = Server.HtmlEncode(String.Format("{0} {1}", asset.Manufacturer, asset.Model).Trim()) : AssetTag.Text = Server.HtmlEncode(asset.AssetTag)
            UserInput.DataSource = New UserRepository().GetActiveUsers() : UserInput.DataTextField = "Label" : UserInput.DataValueField = "Id" : UserInput.DataBind()
            UserInput.Items.Insert(0, New ListItem("Select an employee", String.Empty))
        Catch ex As SqlException
            ShowError("Employees and asset details could not be loaded.") : AssignButton.Enabled = False
        End Try
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class

