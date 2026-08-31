Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class AssetRepository
        Public Function GetAssets(search As String, status As String, categoryId As Integer?) As List(Of AssetListItem)
            Dim sql = New StringBuilder(
                "SELECT a.AssetId, a.AssetTag, c.Name AS CategoryName, a.Manufacturer, a.Model, a.SerialNumber, a.Location, a.Status, a.WarrantyEndDate, " &
                "CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS AssignedToName " &
                "FROM dbo.Assets a INNER JOIN dbo.AssetCategories c ON c.AssetCategoryId = a.AssetCategoryId " &
                "LEFT JOIN dbo.AssetAssignments aa ON aa.AssetId = a.AssetId AND aa.ReturnedAtUtc IS NULL " &
                "LEFT JOIN dbo.Users u ON u.UserId = aa.UserId WHERE 1 = 1 ")
            Dim results As New List(Of AssetListItem)()
            Using connection = Database.CreateConnection(), command = New SqlCommand()
                command.Connection = connection
                If Not String.IsNullOrWhiteSpace(search) Then
                    sql.Append("AND (a.AssetTag LIKE @Search OR a.SerialNumber LIKE @Search OR a.Manufacturer LIKE @Search OR a.Model LIKE @Search) ")
                    command.Parameters.Add("@Search", SqlDbType.NVarChar, 150).Value = "%" & search.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(status) Then
                    sql.Append("AND a.Status = @Status ")
                    command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status
                End If
                If categoryId.HasValue Then
                    sql.Append("AND a.AssetCategoryId = @CategoryId ")
                    command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId.Value
                End If
                sql.Append("ORDER BY a.CreatedAtUtc DESC;")
                command.CommandText = sql.ToString()
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim warrantyOrdinal = reader.GetOrdinal("WarrantyEndDate")
                        results.Add(New AssetListItem With {
                            .AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")), .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")),
                            .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")), .Manufacturer = ReadNullableString(reader, "Manufacturer"),
                            .Model = reader.GetString(reader.GetOrdinal("Model")), .SerialNumber = ReadNullableString(reader, "SerialNumber"),
                            .Location = ReadNullableString(reader, "Location"), .Status = reader.GetString(reader.GetOrdinal("Status")),
                            .AssignedToName = ReadNullableString(reader, "AssignedToName"),
                            .WarrantyEndDate = If(reader.IsDBNull(warrantyOrdinal), CType(Nothing, DateTime?), reader.GetDateTime(warrantyOrdinal))
                        })
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetActiveCategories() As List(Of LookupOption)
            Dim results As New List(Of LookupOption)()
            Const sql = "SELECT AssetCategoryId, Name FROM dbo.AssetCategories WHERE IsActive = 1 ORDER BY Name;"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New LookupOption With {.Id = reader.GetInt32(0), .Label = reader.GetString(1)})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function CreateAsset(categoryId As Integer, assetTag As String, serialNumber As String, manufacturer As String, model As String, purchaseDate As DateTime?, purchaseCost As Decimal?, warrantyEndDate As DateTime?, location As String, status As String, notes As String) As Integer
            Const sql = "INSERT dbo.Assets (AssetCategoryId, AssetTag, SerialNumber, Manufacturer, Model, PurchaseDate, PurchaseCost, WarrantyEndDate, Location, Status, Notes) " &
                        "OUTPUT INSERTED.AssetId VALUES (@CategoryId, @AssetTag, @SerialNumber, @Manufacturer, @Model, @PurchaseDate, @PurchaseCost, @WarrantyEndDate, @Location, @Status, @Notes);"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId
                command.Parameters.Add("@AssetTag", SqlDbType.NVarChar, 40).Value = assetTag.Trim().ToUpperInvariant()
                AddNullableString(command, "@SerialNumber", 100, serialNumber)
                AddNullableString(command, "@Manufacturer", 80, manufacturer)
                command.Parameters.Add("@Model", SqlDbType.NVarChar, 120).Value = model.Trim()
                command.Parameters.Add("@PurchaseDate", SqlDbType.Date).Value = If(purchaseDate.HasValue, CType(purchaseDate.Value.Date, Object), DBNull.Value)
                Dim costParameter = command.Parameters.Add("@PurchaseCost", SqlDbType.Decimal)
                costParameter.Precision = 18 : costParameter.Scale = 2
                costParameter.Value = If(purchaseCost.HasValue, CType(purchaseCost.Value, Object), DBNull.Value)
                command.Parameters.Add("@WarrantyEndDate", SqlDbType.Date).Value = If(warrantyEndDate.HasValue, CType(warrantyEndDate.Value.Date, Object), DBNull.Value)
                AddNullableString(command, "@Location", 150, location)
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status
                AddNullableString(command, "@Notes", 1000, notes)
                connection.Open()
                Return Convert.ToInt32(command.ExecuteScalar())
            End Using
        End Function

        Public Function GetAssetById(assetId As Integer) As AssetDetails
            Const sql = "SELECT a.AssetId, a.AssetTag, c.Name AS CategoryName, a.Manufacturer, a.Model, a.SerialNumber, a.PurchaseDate, a.PurchaseCost, a.WarrantyEndDate, a.Location, a.Status, a.Notes, a.CreatedAtUtc, " &
                        "CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS AssignedToName, aa.AssignedAtUtc " &
                        "FROM dbo.Assets a INNER JOIN dbo.AssetCategories c ON c.AssetCategoryId = a.AssetCategoryId " &
                        "LEFT JOIN dbo.AssetAssignments aa ON aa.AssetId = a.AssetId AND aa.ReturnedAtUtc IS NULL LEFT JOIN dbo.Users u ON u.UserId = aa.UserId WHERE a.AssetId = @AssetId;"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Return New AssetDetails With {
                        .AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")), .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")),
                        .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")), .Manufacturer = ReadNullableString(reader, "Manufacturer"),
                        .Model = reader.GetString(reader.GetOrdinal("Model")), .SerialNumber = ReadNullableString(reader, "SerialNumber"),
                        .PurchaseDate = ReadNullableDate(reader, "PurchaseDate"), .PurchaseCost = ReadNullableDecimal(reader, "PurchaseCost"),
                        .WarrantyEndDate = ReadNullableDate(reader, "WarrantyEndDate"), .Location = ReadNullableString(reader, "Location"),
                        .Status = reader.GetString(reader.GetOrdinal("Status")), .Notes = ReadNullableString(reader, "Notes"),
                        .AssignedToName = ReadNullableString(reader, "AssignedToName"), .AssignedAtUtc = ReadNullableDate(reader, "AssignedAtUtc"),
                        .CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                    }
                End Using
            End Using
        End Function

        Private Shared Sub AddNullableString(command As SqlCommand, name As String, length As Integer, value As String)
            command.Parameters.Add(name, SqlDbType.NVarChar, length).Value = If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value.Trim())
        End Sub
        Private Shared Function ReadNullableString(reader As SqlDataReader, columnName As String) As String
            Dim ordinal = reader.GetOrdinal(columnName) : Return If(reader.IsDBNull(ordinal), String.Empty, reader.GetString(ordinal))
        End Function
        Private Shared Function ReadNullableDate(reader As SqlDataReader, columnName As String) As DateTime?
            Dim ordinal = reader.GetOrdinal(columnName) : Return If(reader.IsDBNull(ordinal), CType(Nothing, DateTime?), reader.GetDateTime(ordinal))
        End Function
        Private Shared Function ReadNullableDecimal(reader As SqlDataReader, columnName As String) As Decimal?
            Dim ordinal = reader.GetOrdinal(columnName) : Return If(reader.IsDBNull(ordinal), CType(Nothing, Decimal?), reader.GetDecimal(ordinal))
        End Function
    End Class
End Namespace
