Imports ITSupportAssetManagement.Web.Security
Public Partial Class ResetPasswordPage
    Inherits System.Web.UI.Page
    Private ReadOnly Property Token As String
        Get
            Return Convert.ToString(Request.QueryString("token"))
        End Get
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Try
            If Not New PasswordResetService().IsValid(Token) Then InvalidPanel.Visible=True : FormPanel.Visible=False
        Catch
            InvalidPanel.Visible = True
            FormPanel.Visible = False
        End Try
    End Sub
    Protected Sub ResetButton_Click(sender As Object, e As EventArgs) Handles ResetButton.Click
        If PasswordInput.Text <> ConfirmInput.Text Then ShowError("The passwords do not match.") : Return
        If Not PasswordPolicy.IsValid(PasswordInput.Text) Then ShowError("Use at least 12 characters with uppercase, lowercase, a number, and a symbol.") : Return
        Try
            If Not New PasswordResetService().ResetPassword(Token, PasswordInput.Text) Then InvalidPanel.Visible=True : FormPanel.Visible=False : Return
            Response.Redirect("~/Login.aspx?reset=1", False)
        Catch
            ShowError("The password could not be reset. Request a new link and try again.")
        End Try
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = message
        ErrorPanel.Visible = True
    End Sub
End Class
