Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data

Public Partial Class TicketCreatePage
    Inherits System.Web.UI.Page
    Private ReadOnly _tickets As New TicketRepository()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then BindLookups()
    End Sub

    Protected Sub CreateButton_Click(sender As Object, e As EventArgs) Handles CreateButton.Click
        If Not Page.IsValid Then Return
        Dim categoryId As Integer
        Dim userId As Integer
        If Not Integer.TryParse(CategoryInput.SelectedValue, categoryId) OrElse Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then
            ShowError("Your session or selected category is invalid. Sign in again and retry.")
            Return
        End If

        Dim assetId As Integer? = Nothing
        Dim parsedAssetId As Integer
        If Integer.TryParse(AssetInput.SelectedValue, parsedAssetId) Then assetId = parsedAssetId

        Try
            Dim ticketNumber = _tickets.CreateTicket(categoryId, assetId, userId, TitleInput.Text, DescriptionInput.Text, PriorityInput.SelectedValue)
            Response.Redirect("~/Tickets/List.aspx?created=" & Server.UrlEncode(ticketNumber), False)
        Catch ex As SqlException
            ShowError("The ticket could not be saved. Verify the database and try again.")
        End Try
    End Sub

    Private Sub BindLookups()
        Try
            CategoryInput.DataSource = _tickets.GetActiveCategories()
            CategoryInput.DataTextField = "Label"
            CategoryInput.DataValueField = "Id"
            CategoryInput.DataBind()
            CategoryInput.Items.Insert(0, New ListItem("Select a category", String.Empty))

            AssetInput.DataSource = _tickets.GetAvailableAssets()
            AssetInput.DataTextField = "Label"
            AssetInput.DataValueField = "Id"
            AssetInput.DataBind()
            AssetInput.Items.Insert(0, New ListItem("No related asset", String.Empty))
        Catch ex As SqlException
            ShowError("Categories and assets could not be loaded. Verify the database connection.")
            CreateButton.Enabled = False
        End Try
    End Sub

    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message)
        ErrorPanel.Visible = True
    End Sub
End Class

