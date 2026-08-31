Option Strict On
Option Explicit On
Partial Public Class MaintenanceListPage
    Protected WithEvents SearchInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents StatusFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents TypeFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents FilterButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents InterventionRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents ResultCount As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents EmptyPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
End Class
