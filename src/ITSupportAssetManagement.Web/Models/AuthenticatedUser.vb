Namespace Models
    Public NotInheritable Class AuthenticatedUser
        Public Property UserId As Integer
        Public Property FirstName As String
        Public Property LastName As String
        Public Property Email As String
        Public Property RoleName As String
        Public Property MustChangePassword As Boolean

        Public ReadOnly Property DisplayName As String
            Get
                Return String.Format("{0} {1}", FirstName, LastName).Trim()
            End Get
        End Property

        Public ReadOnly Property Initials As String
            Get
                Dim firstInitial = If(String.IsNullOrWhiteSpace(FirstName), String.Empty, FirstName.Substring(0, 1))
                Dim lastInitial = If(String.IsNullOrWhiteSpace(LastName), String.Empty, LastName.Substring(0, 1))
                Return (firstInitial & lastInitial).ToUpperInvariant()
            End Get
        End Property
    End Class
End Namespace
