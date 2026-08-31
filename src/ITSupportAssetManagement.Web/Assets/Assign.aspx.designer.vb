Option Strict On
Option Explicit On
Partial Public Class AssetAssignPage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetTag As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents UserInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents UserRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents NotesInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ConfirmInput As Global.System.Web.UI.WebControls.CheckBox
    Protected WithEvents ConfirmValidator As Global.System.Web.UI.WebControls.CustomValidator
    Protected WithEvents AssignButton As Global.System.Web.UI.WebControls.Button
End Class
