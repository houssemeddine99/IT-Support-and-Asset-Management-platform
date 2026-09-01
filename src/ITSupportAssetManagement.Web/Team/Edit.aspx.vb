Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class TeamEditPage
    Inherits System.Web.UI.Page
    Private ReadOnly _users As New UserRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Convert.ToString(Session("RoleName")) <> "Administrator" Then Response.Redirect("~/Default.aspx", False) : Return
        If Not IsPostBack Then LoadForm()
    End Sub
    Private Sub LoadForm()
        Try
            Dim item = _users.GetTeamMemberById(GetId()) : If item Is Nothing Then Response.Redirect("~/Team/List.aspx", False) : Return
            RoleInput.DataSource = _users.GetRoles() : RoleInput.DataTextField = "Label" : RoleInput.DataValueField = "Id" : RoleInput.DataBind() : RoleInput.SelectedValue = item.RoleId.ToString()
            FirstNameInput.Text = item.FirstName : LastNameInput.Text = item.LastName : EmployeeCodeInput.Text = item.EmployeeCode : DepartmentInput.Text = item.Department : EmailInput.Text = item.Email : PhoneInput.Text = item.PhoneNumber
            RoleInput.Enabled = item.UserId <> Convert.ToInt32(Session("UserId"))
        Catch ex As SqlException : ShowError("The team member could not be loaded.") : End Try
    End Sub
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim roleId As Integer, item = _users.GetTeamMemberById(GetId()) : If item Is Nothing Then ShowError("The team member was not found.") : Return
        If item.UserId = Convert.ToInt32(Session("UserId")) Then
            roleId = item.RoleId
        ElseIf Not Integer.TryParse(RoleInput.SelectedValue, roleId) Then
            ShowError("Select a role.") : Return
        End If
        Try : _users.UpdateUser(GetId(), roleId, EmployeeCodeInput.Text, FirstNameInput.Text, LastNameInput.Text, EmailInput.Text, DepartmentInput.Text, PhoneInput.Text) : Response.Redirect("~/Team/List.aspx?updated=1", False)
        Catch ex As Exception When TypeOf ex Is SqlException OrElse TypeOf ex Is InvalidOperationException : ShowError("The account could not be saved. Check whether the email or employee code already exists.") : End Try
    End Sub
    Private Function GetId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
End Class
