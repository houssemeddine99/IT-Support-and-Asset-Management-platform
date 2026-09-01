Namespace Models
    Public NotInheritable Class TicketDetails
        Public Property TicketId As Integer
        Public Property TicketNumber As String
        Public Property TicketCategoryId As Integer
        Public Property AssetId As Integer?
        Public Property Title As String
        Public Property Description As String
        Public Property Priority As String
        Public Property Status As String
        Public Property CategoryName As String
        Public Property RequestedByName As String
        Public Property RequestedByEmail As String
        Public Property AssignedToName As String
        Public Property AssetLabel As String
        Public Property CreatedAtUtc As DateTime
        Public Property DueAtUtc As DateTime?
    End Class
End Namespace
