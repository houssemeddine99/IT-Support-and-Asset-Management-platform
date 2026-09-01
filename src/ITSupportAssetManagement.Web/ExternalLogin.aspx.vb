Imports System.Web.Security
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Security

Public Partial Class ExternalLoginPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Try
            Dim identity As ExternalIdentityService.ExternalIdentity
            Dim service As New ExternalIdentityService()
            If String.IsNullOrWhiteSpace(Request.QueryString("code")) Then
                Response.Redirect(service.CreateAuthorizationUrl(Request, Session), False)
                Context.ApplicationInstance.CompleteRequest()
                Return
            End If
            identity = service.CompleteAuthorization(Request, Session)
            Dim ignoredHash As String = Nothing
            Dim user = New UserRepository().FindActiveByEmail(identity.Email, ignoredHash)
            If user Is Nothing Then RedirectFailure() : Return
            Session("UserId") = user.UserId : Session("DisplayName") = user.DisplayName : Session("Initials") = user.Initials : Session("RoleName") = user.RoleName : Session("MustChangePassword") = user.MustChangePassword
            FormsAuthentication.SetAuthCookie(user.Email, False)
            AuditRepository.Record("SignedIn", "User", user.UserId.ToString(), "Authenticated with Microsoft Entra ID")
            Response.Redirect(If(user.MustChangePassword, "~/Account/Profile.aspx?required=1", "~/Default.aspx"), False)
            Context.ApplicationInstance.CompleteRequest()
        Catch
            RedirectFailure()
        End Try
    End Sub

    Private Sub RedirectFailure()
        Response.Redirect("~/Login.aspx?ssoError=1", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
End Class
