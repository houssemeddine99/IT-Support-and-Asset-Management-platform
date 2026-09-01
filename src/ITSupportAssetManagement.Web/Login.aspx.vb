Imports System.Data.SqlClient
Imports System.Web.Security
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security

Public Partial Class LoginPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.IsAuthenticated Then Response.Redirect("~/Default.aspx", False)
        If Not IsPostBack Then
            If Request.QueryString("passwordChanged") = "1" Then
                SuccessMessage.Text = "Password changed successfully. Sign in with your new password."
                SuccessPanel.Visible = True
            End If
            Dim anyUsers As Boolean = False
            Dim databaseError As SqlException = Nothing
            If TryReadUserState(anyUsers, databaseError) Then
                If Not anyUsers Then Response.Redirect("~/SetupAdmin.aspx", False)
            ElseIf Request.IsLocal Then
                ShowError(String.Format("Database connection failed ({0}): {1}", databaseError.Number, databaseError.Message))
            Else
                ShowError("The database service is temporarily unavailable. Contact IT support.")
            End If
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
            Session("MustChangePassword") = user.MustChangePassword
            If user.MustChangePassword Then
                FormsAuthentication.SetAuthCookie(user.Email, RememberInput.Checked)
                Response.Redirect("~/Account/Profile.aspx?required=1", False)
                Context.ApplicationInstance.CompleteRequest()
            Else
                FormsAuthentication.RedirectFromLoginPage(user.Email, RememberInput.Checked)
            End If
        Catch ex As SqlException
            ShowError("Sign-in is temporarily unavailable. Check the database connection and try again.")
        End Try
    End Sub

    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message)
        ErrorPanel.Visible = True
    End Sub

    Private Shared Function TryReadUserState(ByRef anyUsers As Boolean, ByRef databaseError As SqlException) As Boolean
        For attempt As Integer = 1 To 3
            Try
                anyUsers = New UserRepository().AnyUsers()
                databaseError = Nothing
                Return True
            Catch ex As SqlException
                databaseError = ex
                SqlConnection.ClearAllPools()
                If attempt < 3 Then Threading.Thread.Sleep(attempt * 750)
            End Try
        Next
        Return False
    End Function
End Class
