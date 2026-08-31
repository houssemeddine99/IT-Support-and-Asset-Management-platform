Imports System.Security.Cryptography

Namespace Security
    Public NotInheritable Class PasswordHasher
        Private Const IterationCount As Integer = 120000
        Private Const SaltLength As Integer = 16
        Private Const HashLength As Integer = 32
        Private Const FormatVersion As String = "v1"

        Private Sub New()
        End Sub

        Public Shared Function HashPassword(password As String) As String
            If String.IsNullOrWhiteSpace(password) Then Throw New ArgumentException("A password is required.", NameOf(password))

            Dim salt(SaltLength - 1) As Byte
            Using random = RandomNumberGenerator.Create()
                random.GetBytes(salt)
            End Using

            Dim hash As Byte()
            Using derive = New Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256)
                hash = derive.GetBytes(HashLength)
            End Using

            Return String.Join("$", FormatVersion, IterationCount.ToString(Globalization.CultureInfo.InvariantCulture), Convert.ToBase64String(salt), Convert.ToBase64String(hash))
        End Function

        Public Shared Function VerifyPassword(password As String, storedHash As String) As Boolean
            If String.IsNullOrEmpty(password) OrElse String.IsNullOrEmpty(storedHash) Then Return False

            Try
                Dim parts = storedHash.Split("$"c)
                If parts.Length <> 4 OrElse parts(0) <> FormatVersion Then Return False

                Dim iterations As Integer
                If Not Integer.TryParse(parts(1), iterations) OrElse iterations < 10000 Then Return False

                Dim salt = Convert.FromBase64String(parts(2))
                Dim expected = Convert.FromBase64String(parts(3))
                Dim actual As Byte()

                Using derive = New Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)
                    actual = derive.GetBytes(expected.Length)
                End Using

                Return FixedTimeEquals(actual, expected)
            Catch ex As FormatException
                Return False
            Catch ex As CryptographicException
                Return False
            End Try
        End Function

        Private Shared Function FixedTimeEquals(left As Byte(), right As Byte()) As Boolean
            If left Is Nothing OrElse right Is Nothing OrElse left.Length <> right.Length Then Return False
            Dim difference As Integer = 0
            For index = 0 To left.Length - 1
                difference = difference Or (left(index) Xor right(index))
            Next
            Return difference = 0
        End Function
    End Class
End Namespace

