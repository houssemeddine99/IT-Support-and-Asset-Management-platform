Option Strict On
Option Explicit On

Partial Public Class HomePage
    Protected WithEvents GreetingText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CurrentDateText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ShiftText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents OpenTicketsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AttentionTicketsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TotalAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssignedAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents MaintenanceAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents OverdueMaintenanceText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AverageResolutionText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PriorityTicketRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoPriorityTicketsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents HealthRing As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    Protected WithEvents HealthyPercentageText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents HealthyAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents HealthyLegendPercentage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AttentionAssetsText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AttentionLegendPercentage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents MaintenanceLegendCount As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents MaintenanceLegendPercentage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ActivityRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoActivityPanel As Global.System.Web.UI.WebControls.Panel
End Class
