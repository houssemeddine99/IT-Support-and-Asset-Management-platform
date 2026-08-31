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
    End Class
End Namespace
