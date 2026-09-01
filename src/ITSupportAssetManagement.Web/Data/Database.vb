Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions

Namespace Data
    Public NotInheritable Class Database
        Private Const ConnectionStringName As String = "ITSupportDb"

        Private Sub New()
        End Sub

        Public Shared Function CreateConnection() As SqlConnection
            Dim setting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(ConnectionStringName)

            If setting Is Nothing OrElse String.IsNullOrWhiteSpace(setting.ConnectionString) Then
                Throw New ConfigurationErrorsException(
                    String.Format("Connection string '{0}' is missing or empty.", ConnectionStringName))
            End If

            Return New SqlConnection(ResolveLocalDbConnectionString(setting.ConnectionString))
        End Function

        Private Shared Function ResolveLocalDbConnectionString(connectionString As String) As String
            Dim builder As New SqlConnectionStringBuilder(connectionString)
            Const localDbPrefix As String = "(localdb)\"
            If Not builder.DataSource.StartsWith(localDbPrefix, StringComparison.OrdinalIgnoreCase) Then Return connectionString

            Dim instanceName As String = builder.DataSource.Substring(localDbPrefix.Length)
            If Not Regex.IsMatch(instanceName, "^[A-Za-z0-9_.-]+$") Then Return connectionString

            Dim pipeName As String = ReadInstancePipe(instanceName)
            If String.IsNullOrWhiteSpace(pipeName) Then pipeName = FindActiveDatabasePipe(builder.InitialCatalog)
            If String.IsNullOrWhiteSpace(pipeName) Then
                RunLocalDb("start", instanceName)
                For attempt As Integer = 1 To 20
                    Threading.Thread.Sleep(500)
                    pipeName = ReadInstancePipe(instanceName)
                    If String.IsNullOrWhiteSpace(pipeName) Then pipeName = FindActiveDatabasePipe(builder.InitialCatalog)
                    If Not String.IsNullOrWhiteSpace(pipeName) Then Exit For
                Next
            End If
            If String.IsNullOrWhiteSpace(pipeName) Then Return connectionString

            builder.DataSource = pipeName
            Return builder.ConnectionString
        End Function

        Private Shared Function ReadInstancePipe(instanceName As String) As String
            Dim output As String = RunLocalDb("info", instanceName)
            For Each line As String In output.Split(New Char() {ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
                Const label As String = "Instance pipe name:"
                Dim trimmed As String = line.Trim()
                If trimmed.StartsWith(label, StringComparison.OrdinalIgnoreCase) Then Return trimmed.Substring(label.Length).Trim()
            Next
            Return String.Empty
        End Function

        Private Shared Function FindActiveDatabasePipe(databaseName As String) As String
            Try
                For Each pipePath As String In Directory.GetFiles("\\.\pipe\")
                    If Not Regex.IsMatch(pipePath, "^\\\\\.\\pipe\\LOCALDB#[^\\]+\\tsql\\query$", RegexOptions.IgnoreCase) Then Continue For
                    Dim dataSource As String = "np:" & pipePath
                    Dim probeBuilder As New SqlConnectionStringBuilder With {
                        .DataSource = dataSource,
                        .InitialCatalog = "master",
                        .IntegratedSecurity = True,
                        .ConnectTimeout = 1,
                        .Pooling = False,
                        .TrustServerCertificate = True
                    }
                    Try
                        Using connection As New SqlConnection(probeBuilder.ConnectionString), command As New SqlCommand("SELECT COUNT(1) FROM sys.databases WHERE name=@DatabaseName AND state=0;", connection)
                            command.Parameters.Add("@DatabaseName", System.Data.SqlDbType.NVarChar, 128).Value = databaseName
                            connection.Open()
                            If Convert.ToInt32(command.ExecuteScalar()) = 1 Then Return dataSource
                        End Using
                    Catch ex As SqlException
                        Continue For
                    End Try
                Next
            Catch ex As IOException
                Return String.Empty
            Catch ex As UnauthorizedAccessException
                Return String.Empty
            End Try
            Return String.Empty
        End Function

        Private Shared Function RunLocalDb(action As String, instanceName As String) As String
            Dim executable As String = FindLocalDbExecutable()
            If String.IsNullOrWhiteSpace(executable) Then Return String.Empty

            Dim startInfo As New ProcessStartInfo With {
                .FileName = executable,
                .Arguments = action & " """ & instanceName & """",
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True
            }
            Using process As Process = Process.Start(startInfo)
                If process Is Nothing OrElse Not process.WaitForExit(15000) Then Return String.Empty
                Return process.StandardOutput.ReadToEnd() & Environment.NewLine & process.StandardError.ReadToEnd()
            End Using
        End Function

        Private Shared Function FindLocalDbExecutable() As String
            Dim roots As String() = {
                Environment.GetEnvironmentVariable("ProgramW6432"),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            }
            For Each root As String In roots
                If String.IsNullOrWhiteSpace(root) Then Continue For
                For Each version As String In New String() {"170", "160", "150"}
                    Dim candidate As String = Path.Combine(root, "Microsoft SQL Server", version, "Tools", "Binn", "SqlLocalDB.exe")
                    If File.Exists(candidate) Then Return candidate
                Next
            Next
            Return String.Empty
        End Function
    End Class
End Namespace
