Imports System.Data.SqlClient
Imports System.Globalization
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetCreatePage
    Inherits System.Web.UI.Page
    Private ReadOnly _assets As New AssetRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleName = Convert.ToString(Session("RoleName"))
        If roleName <> "Administrator" AndAlso roleName <> "ITManager" Then Response.Redirect("~/Assets/List.aspx", False) : Return
        If Not IsPostBack Then
            Try
                CategoryInput.DataSource = _assets.GetActiveCategories() : CategoryInput.DataTextField = "Label" : CategoryInput.DataValueField = "Id" : CategoryInput.DataBind()
                CategoryInput.Items.Insert(0, New ListItem("Select a category", String.Empty))
            Catch ex As SqlException
                ShowError("Asset categories could not be loaded.") : CreateButton.Enabled = False
            End Try
        End If
    End Sub
    Protected Sub CreateButton_Click(sender As Object, e As EventArgs) Handles CreateButton.Click
        If Not Page.IsValid Then Return
        Dim categoryId As Integer
        If Not Integer.TryParse(CategoryInput.SelectedValue, categoryId) Then ShowError("Select a valid category.") : Return
        Dim purchaseDate = ParseDate(PurchaseDateInput.Text), warrantyDate = ParseDate(WarrantyEndInput.Text), purchaseCost = ParseDecimal(PurchaseCostInput.Text)
        If Not String.IsNullOrWhiteSpace(PurchaseDateInput.Text) AndAlso Not purchaseDate.HasValue Then ShowError("Purchase date is invalid.") : Return
        If Not String.IsNullOrWhiteSpace(WarrantyEndInput.Text) AndAlso Not warrantyDate.HasValue Then ShowError("Warranty date is invalid.") : Return
        If purchaseDate.HasValue AndAlso warrantyDate.HasValue AndAlso warrantyDate.Value < purchaseDate.Value Then ShowError("Warranty end date cannot be before the purchase date.") : Return
        If Not String.IsNullOrWhiteSpace(PurchaseCostInput.Text) AndAlso Not purchaseCost.HasValue Then ShowError("Purchase cost is invalid.") : Return
        Try
            Dim id = _assets.CreateAsset(categoryId, AssetTagInput.Text, SerialNumberInput.Text, ManufacturerInput.Text, ModelInput.Text, purchaseDate, purchaseCost, warrantyDate, LocationInput.Text, StatusInput.SelectedValue, NotesInput.Text)
            Response.Redirect("~/Assets/Details.aspx?id=" & id.ToString() & "&created=1", False)
        Catch ex As SqlException When ex.Number = 2601 OrElse ex.Number = 2627
            ShowError("The asset tag or serial number is already registered.")
        Catch ex As SqlException
            ShowError("The asset could not be saved. Verify the database and try again.")
        End Try
    End Sub
    Private Shared Function ParseDate(value As String) As DateTime?
        Dim result As DateTime : If String.IsNullOrWhiteSpace(value) Then Return Nothing
        Return If(DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, result), CType(result, DateTime?), Nothing)
    End Function
    Private Shared Function ParseDecimal(value As String) As Decimal?
        Dim result As Decimal : If String.IsNullOrWhiteSpace(value) Then Return Nothing
        Return If(Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, result), CType(result, Decimal?), Nothing)
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
