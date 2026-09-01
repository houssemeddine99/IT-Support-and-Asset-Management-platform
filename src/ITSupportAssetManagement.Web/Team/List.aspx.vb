Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class TeamListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _users As New UserRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role = Convert.ToString(Session("RoleName"))
        If role <> "Administrator" AndAlso role <> "ITManager" Then Response.Redirect("~/Default.aspx", False) : Return
        CreateLink.Visible = role = "Administrator"
        ViewState("CanEdit") = role = "Administrator"
        If Not IsPostBack Then
            If Request.QueryString("created") = "1" Then SuccessMessage.Text = "The team member account was created successfully." : SuccessPanel.Visible = True
            If Request.QueryString("updated") = "1" Then SuccessMessage.Text = "The team member was updated successfully." : SuccessPanel.Visible = True
            BindTeam()
        End If
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
    Protected Function CanManageUser(value As Object) As Boolean
        Return Convert.ToString(Session("RoleName")) = "Administrator" AndAlso Convert.ToInt32(value) <> Convert.ToInt32(Session("UserId"))
    End Function
    Protected Function CanEditUsers() As Boolean
        Return Convert.ToBoolean(ViewState("CanEdit"))
    End Function
    Protected Shared Function GetToggleConfirmation(value As Object) As String
        Return If(Convert.ToBoolean(value), "return confirm('Disable this account?');", String.Empty)
    End Function
    Protected Sub TeamRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles TeamRepeater.ItemCommand
        If e.CommandName <> "ToggleAccount" OrElse Convert.ToString(Session("RoleName")) <> "Administrator" Then Return
        Dim values = Convert.ToString(e.CommandArgument).Split("|"c), userId As Integer, currentUserId = Convert.ToInt32(Session("UserId"))
        If values.Length <> 2 OrElse Not Integer.TryParse(values(0), userId) OrElse userId = currentUserId Then ShowError("You cannot change your own account status.") : Return
        Dim currentlyActive As Boolean
        If Not Boolean.TryParse(values(1), currentlyActive) Then ShowError("The account status is invalid.") : Return
        Try
            _users.SetUserActive(userId, Not currentlyActive)
            AuditRepository.Record(If(currentlyActive, "Disabled", "Activated"), "User", userId.ToString(), If(currentlyActive, "Disabled user account", "Activated user account"))
            SuccessMessage.Text = If(currentlyActive, "The account was disabled successfully.", "The account was activated successfully.") : SuccessPanel.Visible = True : ErrorPanel.Visible = False : BindTeam()
        Catch ex As SqlException
            ShowError("The account status could not be updated.")
        End Try
    End Sub
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True : SuccessPanel.Visible = False
    End Sub
End Class
