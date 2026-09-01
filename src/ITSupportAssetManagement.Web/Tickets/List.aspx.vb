Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data

Public Partial Class TicketListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _tickets As New TicketRepository()
    Private Const PageSize As Integer = 20
    Private Property CurrentPage As Integer
        Get
            Return If(ViewState("CurrentPage") Is Nothing, 0, Convert.ToInt32(ViewState("CurrentPage")))
        End Get
        Set(value As Integer)
            ViewState("CurrentPage") = Math.Max(0, value)
        End Set
    End Property

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
        CurrentPage = 0
        BindTickets()
    End Sub
    Protected Sub PreviousButton_Click(sender As Object, e As EventArgs) Handles PreviousButton.Click
        CurrentPage -= 1 : BindTickets()
    End Sub
    Protected Sub NextButton_Click(sender As Object, e As EventArgs) Handles NextButton.Click
        CurrentPage += 1 : BindTickets()
    End Sub

    Private Sub BindTickets()
        Try
            Dim userId As Integer
            If Not Integer.TryParse(Convert.ToString(Session("UserId")), userId) Then Response.Redirect("~/Login.aspx", False) : Return
            Dim roleName = Convert.ToString(Session("RoleName"))
            Dim canViewAll = roleName = "Administrator" OrElse roleName = "ITManager" OrElse roleName = "Technician"
            Dim totalCount As Integer = 0
            Dim rows = _tickets.GetTickets(SearchInput.Text, StatusFilter.SelectedValue, PriorityFilter.SelectedValue, SlaFilter.SelectedValue, userId, canViewAll, CurrentPage, PageSize, totalCount)
            TicketRepeater.DataSource = rows
            TicketRepeater.DataBind()
            ResultCount.Text = totalCount.ToString()
            Dim pageCount = Math.Max(1, CInt(Math.Ceiling(totalCount / CDbl(PageSize))))
            PageText.Text = "Page " & (CurrentPage + 1).ToString() & " of " & pageCount.ToString()
            PreviousButton.Enabled = CurrentPage > 0 : NextButton.Enabled = CurrentPage + 1 < pageCount
            EmptyPanel.Visible = rows.Count = 0
            ErrorPanel.Visible = False
        Catch ex As SqlException
            ErrorMessage.Text = "Tickets could not be loaded. Verify the database connection."
            ErrorPanel.Visible = True
        End Try
    End Sub
End Class
