Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Mail

Namespace Services
    Public NotInheritable Class EmailService
        Public Sub SendPasswordReset(recipient As String, displayName As String, resetUrl As String)
            Dim fromAddress = ReadSetting("Mail:From", "siliana-it@localhost")
            Using message As New MailMessage(fromAddress, recipient)
                message.Subject = "Reset your Siliana IT Hub password"
                message.IsBodyHtml = True
                message.Body = "<div style='font-family:Arial,sans-serif;color:#101a33'><h2>Password reset requested</h2><p>Hello " & Html(displayName) & ",</p><p>Use the secure link below within 30 minutes. It can be used only once.</p><p><a style='display:inline-block;padding:12px 20px;background:#3156e8;color:#fff;text-decoration:none;border-radius:8px' href='" & Html(resetUrl) & "'>Reset password</a></p><p>If you did not request this, no action is required.</p></div>"
                Using client As New SmtpClient()
                    Dim mode = ReadSetting("Mail:Mode", "Pickup")
                    If mode.Equals("Pickup", StringComparison.OrdinalIgnoreCase) Then
                        Dim pickup = HttpContext.Current.Server.MapPath("~/App_Data/MailPickup")
                        Directory.CreateDirectory(pickup)
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory
                        client.PickupDirectoryLocation = pickup
                    Else
                        client.Host = ReadSetting("Mail:SmtpHost", "localhost")
                        client.Port = Integer.Parse(ReadSetting("Mail:SmtpPort", "587"), Globalization.CultureInfo.InvariantCulture)
                        client.EnableSsl = Boolean.Parse(ReadSetting("Mail:EnableSsl", "true"))
                        Dim username = ConfigurationManager.AppSettings("Mail:Username")
                        If Not String.IsNullOrWhiteSpace(username) Then client.Credentials = New NetworkCredential(username, ConfigurationManager.AppSettings("Mail:Password"))
                    End If
                    client.Send(message)
                End Using
            End Using
        End Sub

        Private Shared Function ReadSetting(key As String, fallback As String) As String
            Dim value = ConfigurationManager.AppSettings(key) : Return If(String.IsNullOrWhiteSpace(value), fallback, value)
        End Function
        Private Shared Function Html(value As String) As String
            Return HttpUtility.HtmlEncode(If(value, String.Empty))
        End Function
    End Class
End Namespace
