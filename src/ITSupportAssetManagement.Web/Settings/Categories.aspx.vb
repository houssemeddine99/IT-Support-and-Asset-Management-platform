Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class CategorySettingsPage
    Inherits System.Web.UI.Page
    Private ReadOnly _settings As New SettingsRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Convert.ToString(Session("RoleName")) <> "Administrator" Then Response.Redirect("~/Default.aspx", False) : Return
        If Not IsPostBack Then BindCategories()
    End Sub
    Private Sub BindCategories()
        Try
            TicketRepeater.DataSource = _settings.GetCategories("ticket") : TicketRepeater.DataBind()
            AssetRepeater.DataSource = _settings.GetCategories("asset") : AssetRepeater.DataBind()
        Catch ex As SqlException
            ShowError("Categories could not be loaded.")
        End Try
    End Sub
    Protected Sub AddTicketButton_Click(sender As Object, e As EventArgs) Handles AddTicketButton.Click
        AddCategory("ticket", TicketNameInput.Text) : TicketNameInput.Text = String.Empty
    End Sub
    Protected Sub AddAssetButton_Click(sender As Object, e As EventArgs) Handles AddAssetButton.Click
        AddCategory("asset", AssetNameInput.Text) : AssetNameInput.Text = String.Empty
    End Sub
    Protected Sub TicketRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles TicketRepeater.ItemCommand
        ToggleCategory("ticket", e)
    End Sub
    Protected Sub AssetRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles AssetRepeater.ItemCommand
        ToggleCategory("asset", e)
    End Sub
    Private Sub AddCategory(kind As String, name As String)
        Try
            _settings.AddCategory(kind, name) : AuditRepository.Record("Category created", "Setting", kind, "Created " & kind & " category " & name.Trim()) : ShowSuccess("The category was added successfully.") : BindCategories()
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("That category already exists or could not be saved.")
        End Try
    End Sub
    Private Sub ToggleCategory(kind As String, e As RepeaterCommandEventArgs)
        If e.CommandName <> "Toggle" Then Return
        Dim id As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then ShowError("The category is invalid.") : Return
        Try
            _settings.ToggleCategory(kind, id) : AuditRepository.Record("Category toggled", "Setting", kind & ":" & id.ToString(), "Changed category availability") : ShowSuccess("The category status was updated.") : BindCategories()
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException
            ShowError("The category status could not be updated.")
        End Try
    End Sub
    Private Sub ShowSuccess(message As String)
        SuccessMessage.Text = Server.HtmlEncode(message) : SuccessPanel.Visible = True : ErrorPanel.Visible = False
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True : SuccessPanel.Visible = False
    End Sub
End Class
