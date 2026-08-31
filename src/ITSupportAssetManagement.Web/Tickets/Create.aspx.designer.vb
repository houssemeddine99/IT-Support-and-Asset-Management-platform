Option Strict On
Option Explicit On
Partial Public Class TicketCreatePage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TitleInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents TitleRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents DescriptionInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents DescriptionRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents CategoryInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents CategoryRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents PriorityInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents AssetInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents CreateButton As Global.System.Web.UI.WebControls.Button
End Class
