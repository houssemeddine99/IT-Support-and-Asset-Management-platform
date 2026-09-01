Namespace Models
    Public NotInheritable Class MaintenanceDetails
        Public Property MaintenanceInterventionId As Integer
        Public Property AssetId As Integer
        Public Property TechnicianUserId As Integer?
        Public Property AssetTag As String
        Public Property AssetName As String
        Public Property AssetLocation As String
        Public Property InterventionType As String
        Public Property Status As String
        Public Property TechnicianName As String
        Public Property Diagnosis As String
        Public Property WorkPerformed As String
        Public Property ScheduledAtUtc As DateTime?
        Public Property StartedAtUtc As DateTime?
        Public Property CompletedAtUtc As DateTime?
        Public Property LaborCost As Decimal?
        Public Property ExternalProvider As String
        Public Property CreatedAtUtc As DateTime
    End Class
End Namespace
