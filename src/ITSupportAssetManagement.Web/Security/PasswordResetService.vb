Imports System.Security.Cryptography
Imports System.Text
Imports ITSupportAssetManagement.Web.Data

Namespace Security
    Public NotInheritable Class PasswordResetService
        Public Function CreateRequest(email As String) As PasswordResetRequest
            Dim tokenBytes(31) As Byte
            Using random = RandomNumberGenerator.Create() : random.GetBytes(tokenBytes) : End Using
            Dim token = Convert.ToBase64String(tokenBytes).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
            Dim user = New UserRepository().CreatePasswordResetToken(email, HashToken(token), DateTime.UtcNow.AddMinutes(30))
            If user Is Nothing Then Return Nothing
            Return New PasswordResetRequest With {.Token=token,.Email=user.Email,.DisplayName=(user.FirstName & " " & user.LastName).Trim()}
        End Function

        Public Function IsValid(token As String) As Boolean
            If String.IsNullOrWhiteSpace(token) OrElse token.Length > 100 Then Return False
            Return New UserRepository().IsPasswordResetTokenValid(HashToken(token))
        End Function

        Public Function ResetPassword(token As String, newPassword As String) As Boolean
            If Not PasswordPolicy.IsValid(newPassword) OrElse String.IsNullOrWhiteSpace(token) Then Return False
            Return New UserRepository().ResetPasswordWithToken(HashToken(token), PasswordHasher.HashPassword(newPassword))
        End Function

        Private Shared Function HashToken(token As String) As Byte()
            Using sha = SHA256.Create() : Return sha.ComputeHash(Encoding.UTF8.GetBytes(token)) : End Using
        End Function
    End Class

    Public NotInheritable Class PasswordResetRequest
        Public Property Token As String
        Public Property Email As String
        Public Property DisplayName As String
    End Class
End Namespace
