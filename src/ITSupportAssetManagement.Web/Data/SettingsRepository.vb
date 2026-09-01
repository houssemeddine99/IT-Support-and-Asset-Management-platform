Imports System.Data
Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class SettingsRepository
        Public Function GetCategories(kind As String) As List(Of CategorySettingItem)
            Dim tableName = GetTableName(kind), idName = If(kind = "ticket", "TicketCategoryId", "AssetCategoryId")
            Dim results As New List(Of CategorySettingItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand("SELECT " & idName & ",Name,IsActive FROM dbo." & tableName & " ORDER BY IsActive DESC,Name;", connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New CategorySettingItem With {.Id = reader.GetInt32(0), .Name = reader.GetString(1), .IsActive = reader.GetBoolean(2)})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Sub AddCategory(kind As String, name As String)
            If String.IsNullOrWhiteSpace(name) Then Throw New InvalidOperationException("Enter a category name.")
            Dim tableName = GetTableName(kind)
            Using connection = Database.CreateConnection(), command As New SqlCommand("INSERT dbo." & tableName & "(Name) VALUES(@Name);", connection)
                command.Parameters.Add("@Name", SqlDbType.NVarChar, 80).Value = name.Trim() : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub ToggleCategory(kind As String, categoryId As Integer)
            Dim tableName = GetTableName(kind), idName = If(kind = "ticket", "TicketCategoryId", "AssetCategoryId")
            Using connection = Database.CreateConnection(), command As New SqlCommand("UPDATE dbo." & tableName & " SET IsActive=CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE " & idName & "=@Id;", connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = categoryId : connection.Open()
                If command.ExecuteNonQuery() = 0 Then Throw New InvalidOperationException("The category was not found.")
            End Using
        End Sub

        Private Shared Function GetTableName(kind As String) As String
            If kind = "ticket" Then Return "TicketCategories"
            If kind = "asset" Then Return "AssetCategories"
            Throw New ArgumentException("Unsupported category type.", NameOf(kind))
        End Function
    End Class
End Namespace
