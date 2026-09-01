Namespace Models
    Public NotInheritable Class MaintenanceListItem
        Public Property MaintenanceInterventionId As Integer
        Public Property AssetId As Integer
        Public Property AssetTag As String
        Public Property AssetName As String
        Public Property InterventionType As String
        Public Property Status As String
        Public Property TechnicianName As String
        Public Property ScheduledAtUtc As DateTime?

        Public ReadOnly Property DisplayStatus As String
            Get
                Return Status.Replace("InProgress", "In progress")
            End Get
        End Property

        Public ReadOnly Property StatusCssClass As String
            Get
                Return "maintenance-state maintenance-" & Status.ToLowerInvariant()
            End Get
        End Property
        Public ReadOnly Property IsOverdue As Boolean
            Get
                Return ScheduledAtUtc.HasValue AndAlso ScheduledAtUtc.Value < DateTime.UtcNow AndAlso (Status = "Planned" OrElse Status = "InProgress")
            End Get
        End Property
    End Class

    Public NotInheritable Class MaintenanceCalendarDay
        Public Property [Date] As DateTime
        Public Property IsCurrentMonth As Boolean
        Public Property IsToday As Boolean
        Public Property Items As New List(Of MaintenanceListItem)()
        Public ReadOnly Property CssClass As String
            Get
                Dim classes As String = "calendar-day"
                If Not IsCurrentMonth Then classes &= " outside-month"
                If IsToday Then classes &= " today"
                If Items.Any(Function(item) item.IsOverdue) Then classes &= " has-overdue"
                Return classes
            End Get
        End Property
    End Class
End Namespace
