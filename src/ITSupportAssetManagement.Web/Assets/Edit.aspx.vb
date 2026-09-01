Imports System.Data.SqlClient
Imports System.Globalization
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetEditPage
    Inherits System.Web.UI.Page
    Private ReadOnly _assets As New AssetRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role = Convert.ToString(Session("RoleName")) : If role <> "Administrator" AndAlso role <> "ITManager" Then Response.Redirect("~/Default.aspx", False) : Return
        Dim id = GetId() : BackLink.NavigateUrl = "Details.aspx?id=" & id.ToString() : CancelLink.NavigateUrl = BackLink.NavigateUrl : If Not IsPostBack Then LoadForm(id)
    End Sub
    Private Sub LoadForm(id As Integer)
        Try
            Dim item = _assets.GetAssetById(id) : If item Is Nothing Then Response.Redirect("~/Assets/List.aspx", False) : Return
            CategoryInput.DataSource = _assets.GetActiveCategories() : CategoryInput.DataTextField = "Label" : CategoryInput.DataValueField = "Id" : CategoryInput.DataBind() : CategoryInput.SelectedValue = item.AssetCategoryId.ToString()
            AssetTagInput.Text = item.AssetTag : ManufacturerInput.Text = item.Manufacturer : ModelInput.Text = item.Model : SerialInput.Text = item.SerialNumber : LocationInput.Text = item.Location : StatusInput.SelectedValue = item.Status : NotesInput.Text = item.Notes
            If item.PurchaseDate.HasValue Then PurchaseDateInput.Text = item.PurchaseDate.Value.ToString("yyyy-MM-dd")
            If item.PurchaseCost.HasValue Then PurchaseCostInput.Text = item.PurchaseCost.Value.ToString(CultureInfo.InvariantCulture)
            If item.WarrantyEndDate.HasValue Then WarrantyInput.Text = item.WarrantyEndDate.Value.ToString("yyyy-MM-dd")
        Catch ex As SqlException : ShowError("The asset could not be loaded.") : End Try
    End Sub
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim categoryId As Integer : If Not Integer.TryParse(CategoryInput.SelectedValue, categoryId) Then ShowError("Select a category.") : Return
        Dim purchaseDate = ParseDate(PurchaseDateInput.Text), warranty = ParseDate(WarrantyInput.Text), cost As Decimal? = Nothing, parsedCost As Decimal
        If Not String.IsNullOrWhiteSpace(PurchaseCostInput.Text) Then
            If Not Decimal.TryParse(PurchaseCostInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, parsedCost) OrElse parsedCost < 0 Then ShowError("Enter a valid purchase cost.") : Return
            cost = parsedCost
        End If
        Try : _assets.UpdateAsset(GetId(), categoryId, AssetTagInput.Text, SerialInput.Text, ManufacturerInput.Text, ModelInput.Text, purchaseDate, cost, warranty, LocationInput.Text, StatusInput.SelectedValue, NotesInput.Text) : Response.Redirect("~/Assets/Details.aspx?id=" & GetId().ToString() & "&updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException : ShowError("The asset could not be saved. Check for duplicate tags or serial numbers.") : End Try
    End Sub
    Private Shared Function ParseDate(text As String) As DateTime?
        Dim value As DateTime
        If DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, value) Then Return value
        Return Nothing
    End Function
    Private Function GetId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
