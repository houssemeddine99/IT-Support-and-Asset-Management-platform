Namespace Models
    Public NotInheritable Class MaintenancePartItem
        Public Property MaintenancePartId As Integer
        Public Property PartName As String
        Public Property PartNumber As String
        Public Property Quantity As Integer
        Public Property UnitCost As Decimal?
        Public ReadOnly Property LineTotal As Decimal
            Get
                Return Quantity * If(UnitCost.HasValue, UnitCost.Value, 0D)
            End Get
        End Property
    End Class
End Namespace
