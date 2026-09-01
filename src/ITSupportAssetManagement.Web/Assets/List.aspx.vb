Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _assets As New AssetRepository()
    Private Const PageSize As Integer = 12
    Private Property CurrentPage As Integer
        Get
            Return If(ViewState("CurrentPage") Is Nothing, 0, Convert.ToInt32(ViewState("CurrentPage")))
        End Get
        Set(value As Integer)
            ViewState("CurrentPage") = Math.Max(0, value)
        End Set
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindCategories()
            If Not String.IsNullOrWhiteSpace(Request.QueryString("created")) Then CreatedAssetTag.Text = Server.HtmlEncode(Request.QueryString("created")) : SuccessPanel.Visible = True
            BindAssets()
        End If
    End Sub
    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        CurrentPage = 0
        BindAssets()
    End Sub
    Protected Sub PreviousButton_Click(sender As Object, e As EventArgs) Handles PreviousButton.Click
        CurrentPage -= 1 : BindAssets()
    End Sub
    Protected Sub NextButton_Click(sender As Object, e As EventArgs) Handles NextButton.Click
        CurrentPage += 1 : BindAssets()
    End Sub
    Private Sub BindCategories()
        CategoryFilter.DataSource = _assets.GetActiveCategories() : CategoryFilter.DataTextField = "Label" : CategoryFilter.DataValueField = "Id" : CategoryFilter.DataBind()
        CategoryFilter.Items.Insert(0, New ListItem("All categories", String.Empty))
    End Sub
    Private Sub BindAssets()
        Try
            Dim categoryId As Integer? = Nothing, parsed As Integer
            If Integer.TryParse(CategoryFilter.SelectedValue, parsed) Then categoryId = parsed
            Dim totalCount As Integer = 0
            Dim rows = _assets.GetAssets(SearchInput.Text, StatusFilter.SelectedValue, categoryId, CurrentPage, PageSize, totalCount)
            AssetRepeater.DataSource = rows : AssetRepeater.DataBind() : ResultCount.Text = totalCount.ToString() : EmptyPanel.Visible = rows.Count = 0 : ErrorPanel.Visible = False
            Dim pageCount = Math.Max(1, CInt(Math.Ceiling(totalCount / CDbl(PageSize))))
            PageText.Text = "Page " & (CurrentPage + 1).ToString() & " of " & pageCount.ToString()
            PreviousButton.Enabled = CurrentPage > 0 : NextButton.Enabled = CurrentPage + 1 < pageCount
        Catch ex As SqlException
            ErrorMessage.Text = "Assets could not be loaded. Verify the database connection." : ErrorPanel.Visible = True
        End Try
    End Sub
    Protected Function DisplayOrDefault(value As Object, fallback As String) As String
        Dim text = Convert.ToString(value)
        Return If(String.IsNullOrWhiteSpace(text), fallback, text)
    End Function
End Class
