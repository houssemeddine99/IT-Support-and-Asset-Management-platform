Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class AssetRepository
        Public Function GetAssets(search As String, status As String, categoryId As Integer?, pageIndex As Integer, pageSize As Integer, ByRef totalCount As Integer) As List(Of AssetListItem)
            Dim sql = New StringBuilder(
                "SELECT a.AssetId, a.AssetTag, c.Name AS CategoryName, a.Manufacturer, a.Model, a.SerialNumber, a.Location, a.Status, a.WarrantyEndDate, " &
                "CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS AssignedToName, COUNT(*) OVER() AS TotalRows " &
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
                sql.Append("ORDER BY a.CreatedAtUtc DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;")
                command.Parameters.Add("@Offset", SqlDbType.Int).Value = Math.Max(0, pageIndex) * pageSize
                command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize
                command.CommandText = sql.ToString()
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        If totalCount = 0 Then totalCount = reader.GetInt32(reader.GetOrdinal("TotalRows"))
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
            Const sql = "SELECT a.AssetId,a.AssetCategoryId,a.AssetTag,c.Name AS CategoryName,a.Manufacturer,a.Model,a.SerialNumber,a.PurchaseDate,a.PurchaseCost,a.WarrantyEndDate,a.Location,a.Status,a.Notes,a.CreatedAtUtc, " &
                        "CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS AssignedToName, aa.AssignedAtUtc " &
                        "FROM dbo.Assets a INNER JOIN dbo.AssetCategories c ON c.AssetCategoryId = a.AssetCategoryId " &
                        "LEFT JOIN dbo.AssetAssignments aa ON aa.AssetId = a.AssetId AND aa.ReturnedAtUtc IS NULL LEFT JOIN dbo.Users u ON u.UserId = aa.UserId WHERE a.AssetId = @AssetId;"
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Return New AssetDetails With {
                        .AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")), .AssetCategoryId = reader.GetInt32(reader.GetOrdinal("AssetCategoryId")), .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")),
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

        Public Function GetAssignmentHistory(assetId As Integer) As List(Of AssetAssignmentHistoryItem)
            Const sql = "SELECT u.FirstName + N' ' + u.LastName AS UserName, assigner.FirstName + N' ' + assigner.LastName AS AssignedByName, aa.AssignedAtUtc, aa.ReturnedAtUtc, aa.AssignmentNotes, aa.ReturnNotes " &
                        "FROM dbo.AssetAssignments aa INNER JOIN dbo.Users u ON u.UserId = aa.UserId INNER JOIN dbo.Users assigner ON assigner.UserId = aa.AssignedByUserId " &
                        "WHERE aa.AssetId = @AssetId ORDER BY aa.AssignedAtUtc DESC;"
            Dim results As New List(Of AssetAssignmentHistoryItem)()
            Using connection = Database.CreateConnection(), command = New SqlCommand(sql, connection)
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New AssetAssignmentHistoryItem With {
                            .UserName = reader.GetString(reader.GetOrdinal("UserName")), .AssignedByName = reader.GetString(reader.GetOrdinal("AssignedByName")),
                            .AssignedAtUtc = reader.GetDateTime(reader.GetOrdinal("AssignedAtUtc")), .ReturnedAtUtc = ReadNullableDate(reader, "ReturnedAtUtc"),
                            .AssignmentNotes = ReadNullableString(reader, "AssignmentNotes"), .ReturnNotes = ReadNullableString(reader, "ReturnNotes")
                        })
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Sub UpdateAsset(assetId As Integer, categoryId As Integer, assetTag As String, serialNumber As String, manufacturer As String, model As String, purchaseDate As DateTime?, purchaseCost As Decimal?, warrantyEndDate As DateTime?, location As String, status As String, notes As String)
            If String.IsNullOrWhiteSpace(assetTag) OrElse String.IsNullOrWhiteSpace(model) Then Throw New InvalidOperationException("Asset tag and model are required.")
            Const sql = "UPDATE dbo.Assets SET AssetCategoryId=@CategoryId,AssetTag=@Tag,SerialNumber=@Serial,Manufacturer=@Manufacturer,Model=@Model,PurchaseDate=@PurchaseDate,PurchaseCost=@Cost,WarrantyEndDate=@Warranty,Location=@Location,Status=@Status,Notes=@Notes,UpdatedAtUtc=SYSUTCDATETIME() WHERE AssetId=@Id; IF @@ROWCOUNT=0 THROW 51000,'Asset not found.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = assetId : command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId : command.Parameters.Add("@Tag", SqlDbType.NVarChar, 40).Value = assetTag.Trim().ToUpperInvariant()
                AddNullableString(command, "@Serial", 100, serialNumber) : AddNullableString(command, "@Manufacturer", 80, manufacturer) : command.Parameters.Add("@Model", SqlDbType.NVarChar, 120).Value = model.Trim()
                command.Parameters.Add("@PurchaseDate", SqlDbType.Date).Value = If(purchaseDate.HasValue, CType(purchaseDate.Value.Date, Object), DBNull.Value)
                Dim cost = command.Parameters.Add("@Cost", SqlDbType.Decimal) : cost.Precision = 18 : cost.Scale = 2 : cost.Value = If(purchaseCost.HasValue, CType(purchaseCost.Value, Object), DBNull.Value)
                command.Parameters.Add("@Warranty", SqlDbType.Date).Value = If(warrantyEndDate.HasValue, CType(warrantyEndDate.Value.Date, Object), DBNull.Value) : AddNullableString(command, "@Location", 150, location)
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status : AddNullableString(command, "@Notes", 1000, notes) : connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Function GetMaintenanceHistory(assetId As Integer) As List(Of MaintenanceListItem)
            Const sql = "SELECT m.MaintenanceInterventionId,m.AssetId,a.AssetTag,LTRIM(RTRIM(COALESCE(a.Manufacturer + N' ',N'') + a.Model)) AS AssetName,m.InterventionType,m.Status,m.ScheduledAtUtc,CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS TechnicianName FROM dbo.MaintenanceInterventions m INNER JOIN dbo.Assets a ON a.AssetId=m.AssetId LEFT JOIN dbo.Users u ON u.UserId=m.TechnicianUserId WHERE m.AssetId=@AssetId ORDER BY COALESCE(m.ScheduledAtUtc,m.CreatedAtUtc) DESC;"
            Dim results As New List(Of MaintenanceListItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New MaintenanceListItem With {
                            .MaintenanceInterventionId = reader.GetInt32(reader.GetOrdinal("MaintenanceInterventionId")), .AssetId = assetId,
                            .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")), .AssetName = reader.GetString(reader.GetOrdinal("AssetName")),
                            .InterventionType = reader.GetString(reader.GetOrdinal("InterventionType")), .Status = reader.GetString(reader.GetOrdinal("Status")),
                            .TechnicianName = ReadNullableString(reader, "TechnicianName"), .ScheduledAtUtc = ReadNullableDate(reader, "ScheduledAtUtc")})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Sub AssignAsset(assetId As Integer, userId As Integer, assignedByUserId As Integer, notes As String)
            Using connection = Database.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction(IsolationLevel.Serializable)
                    Try
                        Const validateSql = "SELECT Status FROM dbo.Assets WITH (UPDLOCK, HOLDLOCK) WHERE AssetId = @AssetId;"
                        Using validate = New SqlCommand(validateSql, connection, transaction)
                            validate.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                            Dim status = Convert.ToString(validate.ExecuteScalar())
                            If String.IsNullOrEmpty(status) Then Throw New InvalidOperationException("The asset was not found.")
                            If status <> "Available" Then Throw New InvalidOperationException("Only an available asset can be assigned.")
                        End Using
                        Const insertSql = "INSERT dbo.AssetAssignments (AssetId, UserId, AssignedByUserId, AssignmentNotes) VALUES (@AssetId, @UserId, @AssignedByUserId, @Notes); UPDATE dbo.Assets SET Status = N'Assigned', UpdatedAtUtc = SYSUTCDATETIME() WHERE AssetId = @AssetId;"
                        Using command = New SqlCommand(insertSql, connection, transaction)
                            command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId
                            command.Parameters.Add("@AssignedByUserId", SqlDbType.Int).Value = assignedByUserId
                            AddNullableString(command, "@Notes", 500, notes)
                            command.ExecuteNonQuery()
                        End Using
                        transaction.Commit()
                    Catch
                        transaction.Rollback() : Throw
                    End Try
                End Using
            End Using
        End Sub

        Public Sub ReturnAsset(assetId As Integer, returnNotes As String)
            Using connection = Database.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction(IsolationLevel.Serializable)
                    Try
                        Const updateSql = "UPDATE dbo.AssetAssignments SET ReturnedAtUtc = SYSUTCDATETIME(), ReturnNotes = @ReturnNotes WHERE AssetId = @AssetId AND ReturnedAtUtc IS NULL; " &
                                          "IF @@ROWCOUNT = 0 THROW 51000, 'No active assignment was found.', 1; " &
                                          "UPDATE dbo.Assets SET Status = N'Available', UpdatedAtUtc = SYSUTCDATETIME() WHERE AssetId = @AssetId;"
                        Using command = New SqlCommand(updateSql, connection, transaction)
                            command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                            AddNullableString(command, "@ReturnNotes", 500, returnNotes)
                            command.ExecuteNonQuery()
                        End Using
                        transaction.Commit()
                    Catch
                        transaction.Rollback() : Throw
                    End Try
                End Using
            End Using
        End Sub

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
