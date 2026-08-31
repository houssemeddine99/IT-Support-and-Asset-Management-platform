Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class TeamListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _users As New UserRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role = Convert.ToString(Session("RoleName"))
        If role <> "Administrator" AndAlso role <> "ITManager" Then Response.Redirect("~/Default.aspx", False) : Return
        CreateLink.Visible = role = "Administrator"
        If Not IsPostBack Then If Request.QueryString("created") = "1" Then SuccessPanel.Visible = True Else BindTeam()
        If Not IsPostBack AndAlso SuccessPanel.Visible Then BindTeam()
    End Sub
    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        BindTeam()
    End Sub
    Private Sub BindTeam()
        Try
            Dim rows = _users.GetTeamMembers(SearchInput.Text, RoleFilter.SelectedValue)
            TeamRepeater.DataSource = rows : TeamRepeater.DataBind() : ResultCount.Text = rows.Count.ToString() : EmptyPanel.Visible = rows.Count = 0
        Catch ex As SqlException
            ErrorMessage.Text = "Team members could not be loaded." : ErrorPanel.Visible = True
        End Try
    End Sub
    Protected Function DisplayOrDefault(value As Object, fallback As String) As String
        Dim text = Convert.ToString(value) : Return If(String.IsNullOrWhiteSpace(text), fallback, text)
    End Function
End Class
