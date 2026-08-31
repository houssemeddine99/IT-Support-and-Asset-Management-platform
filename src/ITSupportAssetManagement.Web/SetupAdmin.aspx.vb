Imports System.Data.SqlClient
Imports System.Linq
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security

Public Partial Class SetupAdminPage
    Inherits System.Web.UI.Page

    Private ReadOnly _users As New UserRepository()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                If _users.AnyUsers() Then Response.Redirect("~/Login.aspx", False)
            Catch ex As SqlException
                ShowError("The database is not ready. Create it and run the initial migration first.")
                CreateButton.Enabled = False
            End Try
        End If
    End Sub

    Protected Sub CreateButton_Click(sender As Object, e As EventArgs) Handles CreateButton.Click
        If Not Page.IsValid Then Return
        If Not IsStrongPassword(PasswordInput.Text) Then
            ShowError("Use at least 12 characters with uppercase, lowercase, number, and symbol.")
            Return
        End If

        Try
            _users.CreateFirstAdministrator(FirstNameInput.Text, LastNameInput.Text, EmailInput.Text, PasswordHasher.HashPassword(PasswordInput.Text))
            Response.Redirect("~/Login.aspx?created=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The administrator could not be created. Verify the database and email address.")
        End Try
    End Sub

    Private Shared Function IsStrongPassword(value As String) As Boolean
        Return value IsNot Nothing AndAlso value.Length >= 12 AndAlso value.Any(AddressOf Char.IsUpper) AndAlso value.Any(AddressOf Char.IsLower) AndAlso value.Any(AddressOf Char.IsDigit) AndAlso value.Any(Function(character) Not Char.IsLetterOrDigit(character))
    End Function

    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message)
        ErrorPanel.Visible = True
    End Sub
End Class
