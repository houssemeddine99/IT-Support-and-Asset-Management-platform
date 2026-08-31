Namespace Models
    Public NotInheritable Class AssetAssignmentHistoryItem
        Public Property UserName As String
        Public Property AssignedByName As String
        Public Property AssignedAtUtc As DateTime
        Public Property ReturnedAtUtc As DateTime?
        Public Property AssignmentNotes As String
        Public Property ReturnNotes As String

        Public ReadOnly Property StatusLabel As String
            Get
                Return If(ReturnedAtUtc.HasValue, "Returned", "Active")
            End Get
        End Property
    End Class
End Namespace

