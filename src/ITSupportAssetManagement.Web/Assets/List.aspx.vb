Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _assets As New AssetRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindCategories()
            If Not String.IsNullOrWhiteSpace(Request.QueryString("created")) Then CreatedAssetTag.Text = Server.HtmlEncode(Request.QueryString("created")) : SuccessPanel.Visible = True
            BindAssets()
        End If
    End Sub
    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        BindAssets()
    End Sub
    Private Sub BindCategories()
        CategoryFilter.DataSource = _assets.GetActiveCategories() : CategoryFilter.DataTextField = "Label" : CategoryFilter.DataValueField = "Id" : CategoryFilter.DataBind()
        CategoryFilter.Items.Insert(0, New ListItem("All categories", String.Empty))
    End Sub
    Private Sub BindAssets()
        Try
            Dim categoryId As Integer? = Nothing, parsed As Integer
            If Integer.TryParse(CategoryFilter.SelectedValue, parsed) Then categoryId = parsed
            Dim rows = _assets.GetAssets(SearchInput.Text, StatusFilter.SelectedValue, categoryId)
            AssetRepeater.DataSource = rows : AssetRepeater.DataBind() : ResultCount.Text = rows.Count.ToString() : EmptyPanel.Visible = rows.Count = 0 : ErrorPanel.Visible = False
        Catch ex As SqlException
            ErrorMessage.Text = "Assets could not be loaded. Verify the database connection." : ErrorPanel.Visible = True
        End Try
    End Sub
    Protected Function DisplayOrDefault(value As Object, fallback As String) As String
        Dim text = Convert.ToString(value)
        Return If(String.IsNullOrWhiteSpace(text), fallback, text)
    End Function
End Class
