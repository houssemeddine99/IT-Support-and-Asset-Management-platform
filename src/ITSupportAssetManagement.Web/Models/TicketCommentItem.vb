Namespace Models
    Public NotInheritable Class TicketCommentItem
        Public Property AuthorName As String
        Public Property Body As String
        Public Property IsInternal As Boolean
        Public Property CreatedAtUtc As DateTime
        Public ReadOnly Property Initials As String
            Get
                Return String.Join(String.Empty, AuthorName.Split(" "c).Where(Function(part) part.Length > 0).Take(2).Select(Function(part) part.Substring(0, 1))).ToUpperInvariant()
            End Get
        End Property
    End Class
End Namespace
