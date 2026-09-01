Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class AuditListPage
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Convert.ToString(Session("RoleName")) <> "Administrator" Then Response.Redirect("~/Default.aspx", False) : Return
        If Not IsPostBack Then BindLogs()
    End Sub
    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        BindLogs()
    End Sub
    Private Sub BindLogs()
        Try
            Dim rows = New AuditRepository().GetLogs(SearchInput.Text, EntityFilter.SelectedValue) : AuditRepeater.DataSource = rows : AuditRepeater.DataBind() : ResultCount.Text = rows.Count.ToString() : EmptyPanel.Visible = rows.Count = 0
        Catch ex As SqlException : ErrorMessage.Text = "Audit activity could not be loaded." : ErrorPanel.Visible = True : End Try
    End Sub
End Class
