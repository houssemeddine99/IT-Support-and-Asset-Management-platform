Option Strict On
Option Explicit On
Partial Public Class AssetDetailsPage
    Protected WithEvents NotFoundPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents DetailsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents StatusBadge As Global.System.Web.UI.WebControls.Label
    Protected WithEvents AssetTag As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents HeroAssetTag As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CategoryName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents SerialNumber As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Location As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PurchaseDate As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PurchaseCost As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents WarrantyEnd As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CreatedDate As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Notes As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssignedPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents AssignedInitials As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssignedName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssignedDate As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents UnassignedPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ScheduleMaintenanceLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents AssignLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents LabelLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents EditLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents ReturnButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents HistoryRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoHistoryPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents MaintenanceListLink As Global.System.Web.UI.WebControls.HyperLink
    Protected WithEvents MaintenanceRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoMaintenancePanel As Global.System.Web.UI.WebControls.Panel
End Class
