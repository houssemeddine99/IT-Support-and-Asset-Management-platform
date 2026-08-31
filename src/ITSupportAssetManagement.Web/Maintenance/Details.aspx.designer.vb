Option Strict On
Option Explicit On
Partial Public Class MaintenanceDetailsPage
    Protected WithEvents NotFoundPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents DetailsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents InterventionNumber As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents StatusBadge As Global.System.Web.UI.WebControls.Label
    Protected WithEvents InterventionType As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents HeadingAsset As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetTag As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetLink As Global.System.Web.UI.HtmlControls.HtmlAnchor
    Protected WithEvents AssetLocation As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Diagnosis As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents WorkPerformed As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CompletionPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents DiagnosisInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents WorkInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents WorkRequired As Global.System.Web.UI.WebControls.RequiredFieldValidator
    Protected WithEvents LaborCostInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents CompleteButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents DetailType As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Technician As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Scheduled As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Started As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Completed As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents Provider As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents LaborCost As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents ActionsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents StartButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents CancelButton As Global.System.Web.UI.WebControls.Button
End Class
