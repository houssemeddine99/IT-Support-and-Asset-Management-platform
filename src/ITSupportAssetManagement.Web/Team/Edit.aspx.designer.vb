Option Strict On
Option Explicit On
Partial Public Class TeamEditPage
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents FirstNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents LastNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmployeeCodeInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents DepartmentInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmailInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PhoneInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents RoleInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents SaveButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents TemporaryPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents TemporaryPasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents ConfirmTemporaryPasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ConfirmTemporaryPasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents TemporaryPasswordMatch As Global.System.Web.UI.WebControls.CompareValidator
    Protected WithEvents ResetPasswordButton As Global.System.Web.UI.WebControls.Button
End Class
