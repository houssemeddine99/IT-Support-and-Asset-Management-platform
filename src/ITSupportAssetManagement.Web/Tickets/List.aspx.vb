Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data

Public Partial Class TicketListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _tickets As New TicketRepository()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If SlaFilter.Items.FindByValue(Request.QueryString("sla")) IsNot Nothing Then SlaFilter.SelectedValue = Request.QueryString("sla")
            If Not String.IsNullOrWhiteSpace(Request.QueryString("created")) Then
                CreatedTicketNumber.Text = Server.HtmlEncode(Request.QueryString("created"))
                SuccessPanel.Visible = True
            End If
            BindTickets()
        End If
    End Sub

    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        BindTickets()
    End Sub

    Private Sub BindTickets()
        Try
            Dim userId As Integer
            If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.Redirect("~/Login.aspx", False) : Return
            Dim roleName = Convert.ToString(Session("RoleName"))
            Dim canViewAll = roleName = "Administrator" OrElse roleName = "ITManager" OrElse roleName = "Technician"
            Dim rows = _tickets.GetTickets(SearchInput.Text, StatusFilter.SelectedValue, PriorityFilter.SelectedValue, SlaFilter.SelectedValue, userId, canViewAll)
            TicketRepeater.DataSource = rows
            TicketRepeater.DataBind()
            ResultCount.Text = rows.Count.ToString()
            EmptyPanel.Visible = rows.Count = 0
            ErrorPanel.Visible = False
        Catch ex As SqlException
            ErrorMessage.Text = "Tickets could not be loaded. Verify the database connection."
            ErrorPanel.Visible = True
        End Try
    End Sub
End Class
