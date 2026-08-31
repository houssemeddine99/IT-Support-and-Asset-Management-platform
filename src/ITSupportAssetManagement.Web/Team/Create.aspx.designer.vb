Option Strict On
Option Explicit On
Partial Public Class TeamCreatePage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents FirstNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents FirstNameRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents LastNameInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents LastNameRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents EmployeeCodeInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents DepartmentInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmailInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents EmailRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PhoneInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents RoleInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents RoleRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PasswordInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PasswordRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents CreateButton As Global.System.Web.UI.WebControls.Button
End Class
