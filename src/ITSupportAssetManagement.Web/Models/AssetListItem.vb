Namespace Models
    Public NotInheritable Class AssetListItem
        Public Property AssetId As Integer
        Public Property AssetTag As String
        Public Property CategoryName As String
        Public Property Manufacturer As String
        Public Property Model As String
        Public Property SerialNumber As String
        Public Property Location As String
        Public Property Status As String
        Public Property AssignedToName As String
        Public Property WarrantyEndDate As DateTime?

        Public ReadOnly Property StatusCssClass As String
            Get
                Return "asset-state asset-" & Status.ToLowerInvariant()
            End Get
        End Property

        Public ReadOnly Property DisplayName As String
            Get
                Return String.Format("{0} {1}", Manufacturer, Model).Trim()
            End Get
        End Property
    End Class
End Namespace

