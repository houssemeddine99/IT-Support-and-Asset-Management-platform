Option Strict On
Option Explicit On
Partial Public Class ReportsOverviewPage
    Protected WithEvents ExportButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TotalTicketsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents OpenTicketsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ResolvedTicketsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ResolutionRateText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TotalAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ActiveMaintenanceText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents MaintenanceCostText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TicketMetricsRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents AssetMetricsRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents MaintenanceMetricsRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoTicketData As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents NoAssetData As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents NoMaintenanceData As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents TicketTrendRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents WorkloadRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoWorkloadData As Global.System.Web.UI.WebControls.Panel
End Class
