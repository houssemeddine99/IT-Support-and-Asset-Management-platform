Imports System.Configuration
Imports System.Data.SqlClient

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

            Return New SqlConnection(setting.ConnectionString)
        End Function
    End Class
End Namespace

