Imports System.Data.SqlClient
Imports System.Linq
Imports System.Web.Security
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security

Public Partial Class AccountProfilePage
    Inherits System.Web.UI.Page

    Private ReadOnly _users As New UserRepository()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then LoadProfile()
    End Sub

    Private Sub LoadProfile()
        Dim userId As Integer
        If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.Redirect("~/Login.aspx", False) : Return
        Try
            Dim user = _users.GetTeamMemberById(userId)
            If user Is Nothing OrElse Not user.IsActive Then FormsAuthentication.SignOut() : Response.Redirect("~/Login.aspx", False) : Return
            InitialsText.Text = Server.HtmlEncode(user.Initials)
            DisplayNameText.Text = Server.HtmlEncode(user.DisplayName)
            RoleText.Text = Server.HtmlEncode(user.RoleName)
            EmailText.Text = Server.HtmlEncode(user.Email)
            EmployeeCodeText.Text = Server.HtmlEncode(DisplayValue(user.EmployeeCode))
            DepartmentText.Text = Server.HtmlEncode(DisplayValue(user.Department))
            PhoneText.Text = Server.HtmlEncode(DisplayValue(user.PhoneNumber))
        Catch ex As SqlException
            ShowError("Your account details could not be loaded.")
            ChangePasswordButton.Enabled = False
        End Try
    End Sub

    Protected Sub ChangePasswordButton_Click(sender As Object, e As EventArgs) Handles ChangePasswordButton.Click
        If Not Page.IsValid Then Return
        If CurrentPasswordInput.Text = NewPasswordInput.Text Then ShowError("Choose a new password that is different from your current password.") : Return
        If Not IsStrongPassword(NewPasswordInput.Text) Then ShowError("Use at least 12 characters with uppercase, lowercase, number, and symbol.") : Return
        Dim userId As Integer
        If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.Redirect("~/Login.aspx", False) : Return
        Try
            If Not New AuthenticationService().ChangePassword(userId, CurrentPasswordInput.Text, NewPasswordInput.Text) Then ShowError("The current password is incorrect.") : Return
            AuditRepository.Record("Changed password", "User", userId.ToString(), "User changed their own account password")
            FormsAuthentication.SignOut() : Session.Clear() : Session.Abandon()
            Response.Redirect("~/Login.aspx?passwordChanged=1", False)
            Context.ApplicationInstance.CompleteRequest()
        Catch ex As SqlException
            ShowError("The password could not be changed. Please try again.")
        End Try
    End Sub

    Private Shared Function IsStrongPassword(value As String) As Boolean
        Return value IsNot Nothing AndAlso value.Length >= 12 AndAlso value.Any(AddressOf Char.IsUpper) AndAlso value.Any(AddressOf Char.IsLower) AndAlso value.Any(AddressOf Char.IsDigit) AndAlso value.Any(Function(character) Not Char.IsLetterOrDigit(character))
    End Function

    Private Shared Function DisplayValue(value As String) As String
        Return If(String.IsNullOrWhiteSpace(value), "Not recorded", value)
    End Function

    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
