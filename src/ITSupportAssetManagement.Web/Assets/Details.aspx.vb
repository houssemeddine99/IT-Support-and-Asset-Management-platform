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
            Dim history = New AssetRepository().GetAssignmentHistory(assetId)
            HistoryRepeater.DataSource = history : HistoryRepeater.DataBind() : NoHistoryPanel.Visible = history.Count = 0
            Dim roleName = Convert.ToString(Session("RoleName")), canManage = roleName = "Administrator" OrElse roleName = "ITManager"
            AssignLink.NavigateUrl = "Assign.aspx?id=" & assetId.ToString() : AssignLink.Visible = canManage AndAlso asset.Status = "Available"
            ReturnButton.Visible = canManage AndAlso asset.Status = "Assigned"
            If Request.QueryString("created") = "1" Then SuccessMessage.Text = "The asset was registered successfully." : SuccessPanel.Visible = True
            If Request.QueryString("assigned") = "1" Then SuccessMessage.Text = "The asset assignment was recorded successfully." : SuccessPanel.Visible = True
            If Request.QueryString("returned") = "1" Then SuccessMessage.Text = "The asset return was recorded successfully." : SuccessPanel.Visible = True
            DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Protected Sub ReturnButton_Click(sender As Object, e As EventArgs) Handles ReturnButton.Click
        Dim roleName = Convert.ToString(Session("RoleName")), assetId As Integer
        If (roleName <> "Administrator" AndAlso roleName <> "ITManager") OrElse Not Integer.TryParse(Request.QueryString("id"), assetId) Then Return
        Try
            Dim repository As New AssetRepository()
            repository.ReturnAsset(assetId, "Returned through the asset details workflow.")
            Response.Redirect("~/Assets/Details.aspx?id=" & assetId.ToString() & "&returned=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException
            ErrorMessage.Text = "The return could not be completed. Refresh the asset and try again." : ErrorPanel.Visible = True
        End Try
    End Sub
    Protected Function DisplayHistoryNotes(value As Object) As String
        Dim text = Convert.ToString(value) : Return If(String.IsNullOrWhiteSpace(text), "No assignment notes", text)
    End Function
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
