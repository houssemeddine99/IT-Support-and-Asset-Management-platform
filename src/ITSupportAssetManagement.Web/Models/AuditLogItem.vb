Namespace Models
    Public NotInheritable Class AuditLogItem
        Public Property AuditLogId As Long
        Public Property ActorName As String
        Public Property ActionName As String
        Public Property EntityType As String
        Public Property EntityKey As String
        Public Property Summary As String
        Public Property IpAddress As String
        Public Property CreatedAtUtc As DateTime
        Public ReadOnly Property Initials As String
            Get
                Dim parts = ActorName.Split(" "c) : Return String.Join(String.Empty, parts.Where(Function(part) part.Length > 0).Take(2).Select(Function(part) part.Substring(0, 1))).ToUpperInvariant()
            End Get
        End Property
    End Class
End Namespace
