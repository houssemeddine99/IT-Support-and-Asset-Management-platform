Imports System.Web.Security
Public Partial Class LogoutPage
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session.Clear()
        Session.Abandon()
        FormsAuthentication.SignOut()
        Response.Redirect("~/Login.aspx", False)
    End Sub
End Class

