Imports ITSupportAssetManagement.Web.Data
Imports ITSupportAssetManagement.Web.Models

Namespace Security
    Public NotInheritable Class AuthenticationService
        Private ReadOnly _users As New UserRepository()

        Public Function Authenticate(email As String, password As String) As AuthenticatedUser
            Dim storedHash As String = Nothing
            Dim user = _users.FindActiveByEmail(email, storedHash)
            If user Is Nothing OrElse Not PasswordHasher.VerifyPassword(password, storedHash) Then Return Nothing
            Return user
        End Function

        Public Function ChangePassword(userId As Integer, currentPassword As String, newPassword As String) As Boolean
            Dim storedHash As String = _users.GetActivePasswordHash(userId)
            If Not PasswordHasher.VerifyPassword(currentPassword, storedHash) Then Return False
            _users.UpdatePassword(userId, PasswordHasher.HashPassword(newPassword))
            Return True
        End Function
    End Class
End Namespace
