Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data

Public Partial Class AssetLabelPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Dim assetId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), assetId) Then ShowNotFound() : Return

        Try
            Dim asset = New AssetRepository().GetAssetById(assetId)
            If asset Is Nothing Then ShowNotFound() : Return
            AssetTag.Text = Server.HtmlEncode(asset.AssetTag)
            AssetName.Text = Server.HtmlEncode(String.Format("{0} {1}", asset.Manufacturer, asset.Model).Trim())
            Location.Text = Server.HtmlEncode(If(String.IsNullOrWhiteSpace(asset.Location), "Location not recorded", asset.Location))
            QrImage.Src = ResolveUrl("~/Assets/QrCode.ashx?id=" & assetId.ToString())
            AssetDetailsLink.HRef = ResolveUrl("~/Assets/Details.aspx?id=" & assetId.ToString())
            LabelPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub

    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True
        LabelPanel.Visible = False
    End Sub
End Class
