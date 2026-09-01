Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Security
Imports ITSupportAssetManagement.Web.Services

Public Partial Class ForgotPasswordPage
    Inherits System.Web.UI.Page
    Protected Sub SendButton_Click(sender As Object, e As EventArgs) Handles SendButton.Click
        If Not Page.IsValid Then Return
        Try
            Dim recoveryRequest = New PasswordResetService().CreateRequest(EmailInput.Text)
            If recoveryRequest IsNot Nothing Then
                Dim resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) & ResolveUrl("~/ResetPassword.aspx?token=" & Server.UrlEncode(recoveryRequest.Token))
                Dim emailSender As New EmailService()
                emailSender.SendPasswordReset(recoveryRequest.Email, recoveryRequest.DisplayName, resetUrl)
            End If
            ResultPanel.Visible = True : SendButton.Enabled = False : EmailInput.Enabled = False
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is Net.Mail.SmtpException OrElse TypeOf ex Is IO.IOException
            ErrorMessage.Text = "Recovery delivery is temporarily unavailable. Contact your IT administrator." : ErrorPanel.Visible = True
        End Try
    End Sub
End Class
