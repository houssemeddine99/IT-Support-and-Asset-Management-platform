Option Strict On
Option Explicit On
Partial Public Class TicketDetailsPage
    Protected WithEvents NotFoundPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents DetailsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents SuccessPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents TicketTitle As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents PriorityBadge As Global.System.Web.UI.WebControls.Label
    Protected WithEvents TicketNumber As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents CreatedDate As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents DescriptionText As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AttachmentRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoAttachmentsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents AttachmentInput As Global.System.Web.UI.WebControls.FileUpload
    Protected WithEvents UploadButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents CommentRepeater As Global.System.Web.UI.WebControls.Repeater
    Protected WithEvents NoCommentsPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents CommentInput As Global.System.Web.UI.WebControls.TextBox
    Protected WithEvents InternalInput As Global.System.Web.UI.WebControls.CheckBox
    Protected WithEvents CommentButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents StatusBadge As Global.System.Web.UI.WebControls.Label
    Protected WithEvents CategoryName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssetLabel As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents AssignedToName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents RequesterInitials As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents RequesterName As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents RequesterEmail As Global.System.Web.UI.WebControls.Literal
    Protected WithEvents WorkflowPanel As Global.System.Web.UI.WebControls.Panel
    Protected WithEvents TechnicianInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents AssignButton As Global.System.Web.UI.WebControls.Button
    Protected WithEvents StatusInput As Global.System.Web.UI.WebControls.DropDownList
    Protected WithEvents StatusButton As Global.System.Web.UI.WebControls.Button
End Class
