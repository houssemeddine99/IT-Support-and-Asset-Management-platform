Option Strict On
Option Explicit On
Partial Public Class MaintenanceCreatePage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents AssetRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents TypeInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents TypeRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents TechnicianInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents ScheduledInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ProviderInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents DiagnosisInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents CreateButton As Global.System.Web.UI.WebControls.Button
End Class
