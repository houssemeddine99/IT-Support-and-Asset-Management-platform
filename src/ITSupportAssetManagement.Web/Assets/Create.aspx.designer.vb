Option Strict On
Option Explicit On
Partial Public Class AssetCreatePage
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetTagInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents AssetTagRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents CategoryInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents CategoryRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents ManufacturerInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ModelInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents ModelRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents SerialNumberInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PurchaseDateInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents PurchaseCostInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents WarrantyEndInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents LocationInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents StatusInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents NotesInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents CreateButton As Global.System.Web.UI.WebControls.Button
End Class
