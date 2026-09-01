Option Strict On
Option Explicit On
Partial Public Class TicketListPage
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents CreatedTicketNumber As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents SearchInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents StatusFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents PriorityFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents SlaFilter As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents FilterButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents TicketRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents EmptyPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ResultCount As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PreviousButton As Global.System.Web.UI.WebControls.LinkButton
    Protected WithEvents PageText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents NextButton As Global.System.Web.UI.WebControls.LinkButton
End Class
