Namespace Models
    Public NotInheritable Class TeamMemberListItem
        Public Property UserId As Integer
        Public Property EmployeeCode As String
        Public Property FirstName As String
        Public Property LastName As String
        Public Property Email As String
        Public Property Department As String
        Public Property PhoneNumber As String
        Public Property RoleName As String
        Public Property IsActive As Boolean
        Public ReadOnly Property DisplayName As String
            Get
                Return FirstName & " " & LastName
            End Get
        End Property
        Public ReadOnly Property Initials As String
            Get
                Return (FirstName.Substring(0, 1) & LastName.Substring(0, 1)).ToUpperInvariant()
            End Get
        End Property
        Public ReadOnly Property StatusText As String
            Get
                Return If(IsActive, "Active", "Inactive")
            End Get
        End Property
    End Class
End Namespace
