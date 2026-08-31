Option Strict On
Option Explicit On
Partial Public Class AssetListPage
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents CreatedAssetTag As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents SearchInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents CategoryFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents StatusFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents FilterButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents AssetRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents EmptyPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ResultCount As Global.System.Web.UI.WebControls.Literal
End Class

