Namespace Models
    Public NotInheritable Class DashboardSnapshot
        Public Property OpenTickets As Integer
        Public Property AttentionTickets As Integer
        Public Property TotalAssets As Integer
        Public Property AssignedAssets As Integer
        Public Property AssetsInMaintenance As Integer
        Public Property OverdueMaintenance As Integer
        Public Property AverageResolutionHours As Decimal
        Public Property HealthyAssets As Integer
        Public Property NeedsAttentionAssets As Integer
        Public Property PriorityTickets As New List(Of DashboardPriorityTicket)()

        Public ReadOnly Property HealthyPercentage As Decimal
            Get
                Return If(TotalAssets = 0, 0D, Math.Round(HealthyAssets * 100D / TotalAssets, 1))
            End Get
        End Property
        Public ReadOnly Property AttentionPercentage As Decimal
            Get
                Return If(TotalAssets = 0, 0D, Math.Round(NeedsAttentionAssets * 100D / TotalAssets, 1))
            End Get
        End Property
        Public ReadOnly Property MaintenancePercentage As Decimal
            Get
                Return If(TotalAssets = 0, 0D, Math.Round(AssetsInMaintenance * 100D / TotalAssets, 1))
            End Get
        End Property
    End Class

    Public NotInheritable Class DashboardPriorityTicket
        Public Property TicketId As Integer
        Public Property TicketNumber As String
        Public Property Title As String
        Public Property Priority As String
        Public Property CategoryName As String
        Public Property AssignedToName As String
        Public Property CreatedAtUtc As DateTime
        Public ReadOnly Property PriorityCss As String
            Get
                Return Priority.ToLowerInvariant()
            End Get
        End Property
        Public ReadOnly Property Initials As String
            Get
                If String.IsNullOrWhiteSpace(AssignedToName) Then Return "--"
                Return String.Join(String.Empty, AssignedToName.Split(" "c).Where(Function(part) part.Length > 0).Take(2).Select(Function(part) part.Substring(0, 1))).ToUpperInvariant()
            End Get
        End Property
    End Class
End Namespace
