Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class TicketAttachmentPage
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim attachmentId As Integer, userId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), attachmentId) OrElse Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.StatusCode = 404 : Return
        Dim role = Convert.ToString(Session("RoleName")), canViewAll = role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician"
        Try
            Dim item = New TicketRepository().GetAttachment(attachmentId, userId, canViewAll)
            If item Is Nothing Then Response.StatusCode = 404 : Return
            Response.Clear() : Response.ContentType = item.ContentType : Response.AddHeader("Content-Disposition", "attachment; filename=""" & item.FileName.Replace("""", String.Empty) & """") : Response.AddHeader("X-Content-Type-Options", "nosniff")
            Response.BinaryWrite(item.FileContent) : Context.ApplicationInstance.CompleteRequest()
        Catch ex As SqlException
            Response.StatusCode = 404
        End Try
    End Sub
End Class
