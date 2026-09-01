Option Strict On
Option Explicit On

Public Partial Class AccountProfilePage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents InitialsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents DisplayNameText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents RoleText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents EmailText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents EmployeeCodeText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents DepartmentText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PhoneText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CurrentPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents CurrentPasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents NewPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents NewPasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents ConfirmPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ConfirmPasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PasswordMatch As Global.System.Web.UI.WebControls.CompareValidator
    Protected WithEvents ChangePasswordButton As Global.System.Web.UI.WebControls.Button
End Class
