Imports System.Web
Imports ITSupportAssetManagement.Web.Data
Imports QRCoder

Public NotInheritable Class AssetQrCodeHandler
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim assetId As Integer
        If Not Integer.TryParse(context.Request.QueryString("id"), assetId) Then
            WriteNotFound(context)
            Return
        End If

        Dim asset = New AssetRepository().GetAssetById(assetId)
        If asset Is Nothing Then
            WriteNotFound(context)
            Return
        End If

        Dim detailsPath = VirtualPathUtility.ToAbsolute("~/Assets/Details.aspx?id=" & assetId.ToString())
        Dim targetUrl = context.Request.Url.GetLeftPart(UriPartial.Authority) & detailsPath
        Dim imageBytes As Byte()
        Using generator As New QRCodeGenerator()
            Using qrData = generator.CreateQrCode(targetUrl, QRCodeGenerator.ECCLevel.Q)
                Dim qrCode As New PngByteQRCode(qrData)
                imageBytes = qrCode.GetGraphic(12)
            End Using
        End Using

        context.Response.Clear()
        context.Response.ContentType = "image/png"
        context.Response.Cache.SetCacheability(HttpCacheability.Private)
        context.Response.Cache.SetMaxAge(TimeSpan.FromMinutes(10))
        context.Response.BinaryWrite(imageBytes)
    End Sub

    Private Shared Sub WriteNotFound(context As HttpContext)
        context.Response.StatusCode = 404
        context.Response.ContentType = "text/plain"
        context.Response.Write("Asset not found.")
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
