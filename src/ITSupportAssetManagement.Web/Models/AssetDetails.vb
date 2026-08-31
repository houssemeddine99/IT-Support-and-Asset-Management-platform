Namespace Models
    Public NotInheritable Class AssetDetails
        Public Property AssetId As Integer
        Public Property AssetTag As String
        Public Property CategoryName As String
        Public Property Manufacturer As String
        Public Property Model As String
        Public Property SerialNumber As String
        Public Property PurchaseDate As DateTime?
        Public Property PurchaseCost As Decimal?
        Public Property WarrantyEndDate As DateTime?
        Public Property Location As String
        Public Property Status As String
        Public Property Notes As String
        Public Property AssignedToName As String
        Public Property AssignedAtUtc As DateTime?
        Public Property CreatedAtUtc As DateTime
    End Class
End Namespace

