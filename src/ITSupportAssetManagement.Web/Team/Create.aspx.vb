Imports System.Data.SqlClient
Imports System.Linq
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security
Public Partial Class TeamCreatePage
    Inherits System.Web.UI.Page
    Private ReadOnly _users As New UserRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Convert.ToString(Session("RoleName")) <> "Administrator" Then Response.Redirect("~/Team/List.aspx", False) : Return
        If Not IsPostBack Then
            Try
                RoleInput.DataSource = _users.GetRoles() : RoleInput.DataTextField = "Label" : RoleInput.DataValueField = "Id" : RoleInput.DataBind() : RoleInput.Items.Insert(0, New ListItem("Select a role", String.Empty))
            Catch ex As SqlException
                ShowError("Roles could not be loaded.") : CreateButton.Enabled = False
            End Try
        End If
    End Sub
    Protected Sub CreateButton_Click(sender As Object, e As EventArgs) Handles CreateButton.Click
        If Not Page.IsValid Then Return
        Dim roleId As Integer
        If Not Integer.TryParse(RoleInput.SelectedValue, roleId) Then ShowError("Select a valid role.") : Return
        If Not IsStrongPassword(PasswordInput.Text) Then ShowError("Use at least 12 characters with uppercase, lowercase, number, and symbol.") : Return
        Try
            _users.CreateUser(roleId, EmployeeCodeInput.Text, FirstNameInput.Text, LastNameInput.Text, EmailInput.Text, PasswordHasher.HashPassword(PasswordInput.Text), DepartmentInput.Text, PhoneInput.Text)
            Response.Redirect("~/Team/List.aspx?created=1", False)
        Catch ex As SqlException When ex.Number = 2601 OrElse ex.Number = 2627
            ShowError("The email address or employee code is already registered.")
        Catch ex As SqlException
            ShowError("The account could not be created.")
        End Try
    End Sub
    Private Shared Function IsStrongPassword(value As String) As Boolean
        Return value IsNot Nothing AndAlso value.Length >= 12 AndAlso value.Any(AddressOf Char.IsUpper) AndAlso value.Any(AddressOf Char.IsLower) AndAlso value.Any(AddressOf Char.IsDigit) AndAlso value.Any(Function(character) Not Char.IsLetterOrDigit(character))
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
