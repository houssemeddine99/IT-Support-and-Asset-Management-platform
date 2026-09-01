Namespace Models
    Public NotInheritable Class CategorySettingItem
        Public Property Id As Integer
        Public Property Name As String
        Public Property IsActive As Boolean
        Public ReadOnly Property StatusText As String
            Get
                Return If(IsActive, "Active", "Inactive")
            End Get
        End Property
    End Class
End Namespace
