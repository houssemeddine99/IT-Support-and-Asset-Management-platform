Option Strict On
Option Explicit On

Partial Public Class SiteMaster
    Protected WithEvents HeadContent As Global.System.Web.UI.WebControls.ContentPlaceHolder
    Protected WithEvents MainForm As Global.System.Web.UI.HtmlControls.HtmlForm
    Protected WithEvents MainContent As Global.System.Web.UI.WebControls.ContentPlaceHolder
    Protected WithEvents GlobalSearchPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents GlobalSearchInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents GlobalSearchButton As Global.System.Web.UI.WebControls.LinkButton
    Protected WithEvents OpenTicketCount As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    Protected WithEvents AssetCapacityPercentage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetCapacityBar As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    Protected WithEvents AssetCapacityDetail As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents UserInitials As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents UserDisplayName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents UserRole As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents NotificationCount As Global.System.Web.UI.WebControls.Label
End Class
