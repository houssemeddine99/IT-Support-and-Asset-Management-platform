Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AssetDetailsPage
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Dim assetId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), assetId) Then ShowNotFound() : Return
        Try
            Dim asset = New AssetRepository().GetAssetById(assetId)
            If asset Is Nothing Then ShowNotFound() : Return
            AssetName.Text = Server.HtmlEncode(String.Format("{0} {1}", asset.Manufacturer, asset.Model).Trim())
            AssetTag.Text = Server.HtmlEncode(asset.AssetTag) : HeroAssetTag.Text = AssetTag.Text : CategoryName.Text = Server.HtmlEncode(asset.CategoryName)
            StatusBadge.Text = asset.Status.Replace("InMaintenance", "In maintenance") : StatusBadge.CssClass = "asset-state asset-" & asset.Status.ToLowerInvariant()
            SerialNumber.Text = DisplayValue(asset.SerialNumber) : Location.Text = DisplayValue(asset.Location)
            PurchaseDate.Text = DisplayDate(asset.PurchaseDate) : WarrantyEnd.Text = DisplayDate(asset.WarrantyEndDate)
            PurchaseCost.Text = If(asset.PurchaseCost.HasValue, asset.PurchaseCost.Value.ToString("N2") & " TND", "Not recorded")
            CreatedDate.Text = asset.CreatedAtUtc.ToLocalTime().ToString("dd MMM yyyy") : Notes.Text = DisplayValue(asset.Notes)
            If String.IsNullOrWhiteSpace(asset.AssignedToName) Then
                UnassignedPanel.Visible = True
            Else
                AssignedName.Text = Server.HtmlEncode(asset.AssignedToName)
                AssignedDate.Text = If(asset.AssignedAtUtc.HasValue, asset.AssignedAtUtc.Value.ToLocalTime().ToString("dd MMM yyyy"), "unknown")
                Dim names = asset.AssignedToName.Split(" "c) : AssignedInitials.Text = Server.HtmlEncode(String.Join(String.Empty, names.Where(Function(value) value.Length > 0).Take(2).Select(Function(value) value.Substring(0, 1))).ToUpperInvariant())
                AssignedPanel.Visible = True
            End If
            SuccessPanel.Visible = Request.QueryString("created") = "1" : DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Private Function DisplayValue(value As String) As String
        Return Server.HtmlEncode(If(String.IsNullOrWhiteSpace(value), "Not recorded", value))
    End Function
    Private Shared Function DisplayDate(value As DateTime?) As String
        Return If(value.HasValue, value.Value.ToString("dd MMM yyyy"), "Not recorded")
    End Function
    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True : DetailsPanel.Visible = False
    End Sub
End Class

