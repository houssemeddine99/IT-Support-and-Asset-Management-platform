Imports System.Data.SqlClient
Imports System.Web.Security
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security

Public Partial Class LoginPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.IsAuthenticated Then Response.Redirect("~/Default.aspx", False)
        If Not IsPostBack Then
            Try
                If Not New UserRepository().AnyUsers() Then Response.Redirect("~/SetupAdmin.aspx", False)
            Catch ex As SqlException
                ShowError("The database is not ready. Create it and run the initial migration first.")
            End Try
        End If
    End Sub

    Protected Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        If Not Page.IsValid Then Return
        Try
            Dim user = New AuthenticationService().Authenticate(EmailInput.Text, PasswordInput.Text)
            If user Is Nothing Then
                ShowError("The email or password is incorrect.")
                Return
            End If

            Session("UserId") = user.UserId
            Session("DisplayName") = user.DisplayName
            Session("Initials") = user.Initials
            Session("RoleName") = user.RoleName
            FormsAuthentication.RedirectFromLoginPage(user.Email, RememberInput.Checked)
        Catch ex As SqlException
            ShowError("Sign-in is temporarily unavailable. Check the database connection and try again.")
        End Try
    End Sub

    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message)
        ErrorPanel.Visible = True
    End Sub
End Class

