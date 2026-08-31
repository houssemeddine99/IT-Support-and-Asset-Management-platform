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
    End Class
End Namespace

