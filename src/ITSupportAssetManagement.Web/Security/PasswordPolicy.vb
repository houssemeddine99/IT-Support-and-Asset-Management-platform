Namespace Security
    Public NotInheritable Class PasswordPolicy
        Public Shared Function IsValid(value As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(value) AndAlso value.Length >= 12 AndAlso value.Any(AddressOf Char.IsUpper) AndAlso value.Any(AddressOf Char.IsLower) AndAlso value.Any(AddressOf Char.IsDigit) AndAlso value.Any(Function(c) Not Char.IsLetterOrDigit(c))
        End Function
    End Class
End Namespace
