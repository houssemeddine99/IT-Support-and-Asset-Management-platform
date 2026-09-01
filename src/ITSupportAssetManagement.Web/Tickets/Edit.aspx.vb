Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class TicketEditPage
    Inherits System.Web.UI.Page
    Private ReadOnly _tickets As New TicketRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsStaff() Then Response.Redirect("~/Default.aspx", False) : Return
        Dim id = GetId() : BackLink.NavigateUrl = "Details.aspx?id=" & id.ToString() : CancelLink.NavigateUrl = BackLink.NavigateUrl
        If Not IsPostBack Then LoadForm(id)
    End Sub
    Private Sub LoadForm(id As Integer)
        Try
            Dim item = _tickets.GetTicketById(id, Convert.ToInt32(Session("UserId")), True) : If item Is Nothing Then Response.Redirect("~/Tickets/List.aspx", False) : Return
            CategoryInput.DataSource = _tickets.GetActiveCategories() : CategoryInput.DataTextField = "Label" : CategoryInput.DataValueField = "Id" : CategoryInput.DataBind()
            AssetInput.DataSource = _tickets.GetAvailableAssets() : AssetInput.DataTextField = "Label" : AssetInput.DataValueField = "Id" : AssetInput.DataBind() : AssetInput.Items.Insert(0, New ListItem("No related asset", String.Empty))
            CategoryInput.SelectedValue = item.TicketCategoryId.ToString() : If item.AssetId.HasValue AndAlso AssetInput.Items.FindByValue(item.AssetId.Value.ToString()) IsNot Nothing Then AssetInput.SelectedValue = item.AssetId.Value.ToString()
            PriorityInput.SelectedValue = item.Priority : TitleInput.Text = item.Title : DescriptionInput.Text = item.Description
        Catch ex As SqlException : ShowError("The ticket could not be loaded.") : End Try
    End Sub
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim categoryId As Integer, assetValue As Integer, assetId As Integer? = Nothing : If Not Integer.TryParse(CategoryInput.SelectedValue, categoryId) Then ShowError("Select a category.") : Return
        If Integer.TryParse(AssetInput.SelectedValue, assetValue) Then assetId = assetValue
        Try : _tickets.UpdateTicket(GetId(), categoryId, assetId, TitleInput.Text, DescriptionInput.Text, PriorityInput.SelectedValue) : AuditRepository.Record("Edited", "Ticket", GetId().ToString(), "Updated ticket details and priority") : Response.Redirect("~/Tickets/Details.aspx?id=" & GetId().ToString() & "&updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException : ShowError("The ticket could not be saved. Verify all fields.") : End Try
    End Sub
    Private Function GetId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Function IsStaff() As Boolean
        Dim role = Convert.ToString(Session("RoleName")) : Return role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician"
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
