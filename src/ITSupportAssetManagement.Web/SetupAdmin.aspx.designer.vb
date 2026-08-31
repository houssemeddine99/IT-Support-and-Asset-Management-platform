Option Strict On
Option Explicit On
Partial Public Class SetupAdminPage
    Protected WithEvents SetupForm As Global.System.Web.UI.HtmlControls.HtmlForm
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents FirstNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents FirstNameRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents LastNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents LastNameRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents EmailInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmailRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents ConfirmPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PasswordMatch As Global.System.Web.UI.WebControls.CompareValidator
    Protected WithEvents CreateButton As Global.System.Web.UI.WebControls.Button
End Class

