Imports System.Data.SqlClient

Public Partial Class SearchIndex
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Dim query As String = Convert.ToString(Request.QueryString("q")).Trim()
        If query.Length > 100 Then query = query.Substring(0, 100)
        QueryText.Text = Server.HtmlEncode(query)
        If query.Length < 2 Then
            ShowEmpty("Enter at least two characters in the search box.")
            Return
        End If

        Dim userId As Integer
        If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.Redirect("~/Login.aspx") : Return
        Dim role As String = Convert.ToString(Session("RoleName"))
        Dim canSeeAll As Boolean = role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician"
        Try
            Dim items As List(Of Models.GlobalSearchResult) = New Data.SearchRepository().Search(query, userId, canSeeAll)
            ResultRepeater.DataSource = items : ResultRepeater.DataBind()
            ResultCount.Text = items.Count.ToString()
            EmptyPanel.Visible = items.Count = 0
        Catch ex As SqlException
            AlertText.Text = "Search is temporarily unavailable. Please try again."
            AlertPanel.Visible = True : ResultCount.Text = "0" : EmptyPanel.Visible = True
        End Try
    End Sub

    Private Sub ShowEmpty(message As String)
        ResultCount.Text = "0" : EmptyPanel.Visible = True
        AlertText.Text = Server.HtmlEncode(message) : AlertPanel.Visible = True
    End Sub

    Protected Function ResultIcon(value As Object) As String
        Select Case Convert.ToString(value)
            Case "Ticket" : Return "bi bi-inbox"
            Case "Asset" : Return "bi bi-laptop"
            Case Else : Return "bi bi-person"
        End Select
    End Function

    Protected Function ResultClass(value As Object) As String
        Return Convert.ToString(value).ToLowerInvariant()
    End Function

    Protected Function FormatResultDate(value As Object) As String
        Return DirectCast(value, DateTime).ToLocalTime().ToString("dd MMM yyyy")
    End Function
End Class
