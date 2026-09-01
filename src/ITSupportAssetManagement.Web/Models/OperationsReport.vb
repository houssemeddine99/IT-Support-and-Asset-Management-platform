Namespace Models
    Public NotInheritable Class OperationsReport
        Public Property TotalTickets As Integer
        Public Property ResolvedTickets As Integer
        Public Property OpenTickets As Integer
        Public Property TotalAssets As Integer
        Public Property ActiveMaintenance As Integer
        Public Property TotalMaintenanceCost As Decimal
        Public Property TicketsByStatus As New List(Of ReportMetric)()
        Public Property AssetsByStatus As New List(Of ReportMetric)()
        Public Property MaintenanceByType As New List(Of ReportMetric)()
        Public Property TicketTrend As New List(Of ReportTrendPoint)()
        Public Property TechnicianWorkload As New List(Of TechnicianWorkloadItem)()
        Public ReadOnly Property ResolutionRate As Decimal
            Get
                Return If(TotalTickets = 0, 0D, Math.Round(ResolvedTickets * 100D / TotalTickets, 1))
            End Get
        End Property
    End Class
    Public NotInheritable Class ReportTrendPoint
        Public Property MonthLabel As String
        Public Property CreatedCount As Integer
        Public Property ResolvedCount As Integer
        Public Property CreatedHeight As Decimal
        Public Property ResolvedHeight As Decimal
        Public ReadOnly Property CreatedStyle As String
            Get
                Return "height:" & CreatedHeight.ToString("0.#", Globalization.CultureInfo.InvariantCulture) & "%"
            End Get
        End Property
        Public ReadOnly Property ResolvedStyle As String
            Get
                Return "height:" & ResolvedHeight.ToString("0.#", Globalization.CultureInfo.InvariantCulture) & "%"
            End Get
        End Property
    End Class
    Public NotInheritable Class TechnicianWorkloadItem
        Public Property DisplayName As String
        Public Property OpenTickets As Integer
        Public Property ActiveMaintenance As Integer
        Public Property LoadPercentage As Decimal
        Public ReadOnly Property TotalWork As Integer
            Get
                Return OpenTickets + ActiveMaintenance
            End Get
        End Property
        Public ReadOnly Property LoadStyle As String
            Get
                Return "width:" & LoadPercentage.ToString("0.#", Globalization.CultureInfo.InvariantCulture) & "%"
            End Get
        End Property
    End Class
    Public NotInheritable Class ReportMetric
        Public Property Label As String
        Public Property Value As Integer
        Public Property Percentage As Decimal
        Public ReadOnly Property BarStyle As String
            Get
                Return "width:" & Percentage.ToString("0.#", Globalization.CultureInfo.InvariantCulture) & "%"
            End Get
        End Property
    End Class
End Namespace
