Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Models
Imports ITSupportAssetManagement.Web.Security

Public Partial Class MaintenanceCalendarPage
    Inherits System.Web.UI.Page
    Private ReadOnly _maintenance As New MaintenanceRepository()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then BindCalendar()
    End Sub

    Private Sub BindCalendar()
        Dim month As DateTime = ReadMonth()
        Dim nextMonth As DateTime = month.AddMonths(1), gridStart As DateTime = month.AddDays(-CInt(month.DayOfWeek)), gridEnd As DateTime = gridStart.AddDays(42)
        MonthTitle.Text = month.ToString("MMMM yyyy")
        PreviousMonthLink.HRef = "Calendar.aspx?month=" & month.AddMonths(-1).ToString("yyyy-MM")
        NextMonthLink.HRef = "Calendar.aspx?month=" & nextMonth.ToString("yyyy-MM")
        PlanAction.Visible = AuthorizationService.CanExecuteMaintenance(Convert.ToString(Session("RoleName")))
        Try
            Dim rows = _maintenance.GetCalendarInterventions(gridStart.ToUniversalTime(), gridEnd.ToUniversalTime())
            Dim days As New List(Of MaintenanceCalendarDay)()
            For offset As Integer = 0 To 41
                Dim current As DateTime = gridStart.AddDays(offset)
                Dim day As New MaintenanceCalendarDay With {.[Date]=current,.IsCurrentMonth=current.Month=month.Month,.IsToday=current.Date=DateTime.Today}
                day.Items.AddRange(rows.Where(Function(item) item.ScheduledAtUtc.HasValue AndAlso item.ScheduledAtUtc.Value.ToLocalTime().Date=current.Date))
                days.Add(day)
            Next
            DayRepeater.DataSource = days : DayRepeater.DataBind() : ScheduledCount.Text = rows.Count.ToString()
        Catch ex As SqlException
            ErrorMessage.Text = "The maintenance calendar could not be loaded." : ErrorPanel.Visible = True
        End Try
    End Sub

    Private Function ReadMonth() As DateTime
        Dim parsed As DateTime
        If DateTime.TryParseExact(Request.QueryString("month"), "yyyy-MM", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, parsed) AndAlso parsed.Year >= 2000 AndAlso parsed.Year <= 2100 Then Return New DateTime(parsed.Year, parsed.Month, 1)
        Return New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
    End Function

    Protected Function DisplayEventTime(value As Object) As String
        Return DirectCast(value, DateTime).ToLocalTime().ToString("HH:mm")
    End Function

    Protected Function EventClass(value As Object) As String
        Dim item As MaintenanceListItem = DirectCast(value, MaintenanceListItem)
        Return "calendar-event " & If(item.IsOverdue, "overdue", item.Status.ToLowerInvariant())
    End Function
End Class
