Namespace Models
    Public NotInheritable Class TicketListItem
        Public Property TicketId As Integer
        Public Property TicketNumber As String
        Public Property Title As String
        Public Property Priority As String
        Public Property Status As String
        Public Property CategoryName As String
        Public Property RequestedByName As String
        Public Property AssignedToName As String
        Public Property AssetTag As String
        Public Property CreatedAtUtc As DateTime
        Public Property DueAtUtc As DateTime?

        Public ReadOnly Property PriorityCssClass As String
            Get
                Return "status status-" & Priority.ToLowerInvariant()
            End Get
        End Property

        Public ReadOnly Property StatusCssClass As String
            Get
                Return "ticket-state state-" & Status.ToLowerInvariant()
            End Get
        End Property
        Public ReadOnly Property SlaLabel As String
            Get
                If Not DueAtUtc.HasValue OrElse Status = "Resolved" OrElse Status = "Closed" OrElse Status = "Cancelled" Then Return "No active SLA"
                Dim remaining = DueAtUtc.Value - DateTime.UtcNow
                If remaining.TotalMinutes < 0 Then Return "Overdue " & FormatDuration(remaining.Negate())
                Return "Due in " & FormatDuration(remaining)
            End Get
        End Property
        Public ReadOnly Property SlaCssClass As String
            Get
                If Not DueAtUtc.HasValue OrElse Status = "Resolved" OrElse Status = "Closed" OrElse Status = "Cancelled" Then Return "sla-chip neutral"
                If DueAtUtc.Value < DateTime.UtcNow Then Return "sla-chip overdue"
                If DueAtUtc.Value <= DateTime.UtcNow.AddHours(4) Then Return "sla-chip warning"
                Return "sla-chip healthy"
            End Get
        End Property
        Private Shared Function FormatDuration(value As TimeSpan) As String
            If value.TotalDays >= 1 Then Return Math.Floor(value.TotalDays).ToString("0") & "d " & value.Hours.ToString() & "h"
            If value.TotalHours >= 1 Then Return Math.Floor(value.TotalHours).ToString("0") & "h " & value.Minutes.ToString() & "m"
            Return Math.Max(1, value.Minutes).ToString() & "m"
        End Function
    End Class
End Namespace
