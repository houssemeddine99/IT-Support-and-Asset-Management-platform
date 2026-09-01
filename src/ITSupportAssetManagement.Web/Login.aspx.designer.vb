Option Strict On
Option Explicit On
Partial Public Class LoginPage
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents LoginForm As Global.System.Web.UI.HtmlControls.HtmlForm
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CompanySignInPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents CompanySignInLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents EmailInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmailRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents RememberInput As Global.System.Web.UI.WebControls.CheckBox
    Protected WithEvents LoginButton As Global.System.Web.UI.WebControls.Button
End Class
