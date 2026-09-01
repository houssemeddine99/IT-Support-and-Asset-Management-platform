Option Strict On
Option Explicit On
Partial Public Class MaintenanceCalendarPage
    Protected WithEvents PlanAction As Global.System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PreviousMonthLink As Global.System.Web.UI.HtmlControls.HtmlAnchor
    Protected WithEvents NextMonthLink As Global.System.Web.UI.HtmlControls.HtmlAnchor
    Protected WithEvents MonthTitle As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents DayRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents ScheduledCount As Global.System.Web.UI.WebControls.Literal
End Class
