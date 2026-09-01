Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports ITSupportAssetManagement.Web.Models

Namespace Data
    Public NotInheritable Class MaintenanceRepository
        Public Function GetInterventions(search As String, status As String, interventionType As String) As List(Of MaintenanceListItem)
            Dim sql As New StringBuilder("SELECT m.MaintenanceInterventionId, m.AssetId, a.AssetTag, LTRIM(RTRIM(COALESCE(a.Manufacturer + N' ', N'') + a.Model)) AS AssetName, m.InterventionType, m.Status, m.ScheduledAtUtc, CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS TechnicianName FROM dbo.MaintenanceInterventions m INNER JOIN dbo.Assets a ON a.AssetId = m.AssetId LEFT JOIN dbo.Users u ON u.UserId = m.TechnicianUserId WHERE 1=1 ")
            Dim results As New List(Of MaintenanceListItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand()
                command.Connection = connection
                If Not String.IsNullOrWhiteSpace(search) Then
                    sql.Append("AND (a.AssetTag LIKE @Search OR a.Manufacturer LIKE @Search OR a.Model LIKE @Search OR u.FirstName LIKE @Search OR u.LastName LIKE @Search) ")
                    command.Parameters.Add("@Search", SqlDbType.NVarChar, 150).Value = "%" & search.Trim() & "%"
                End If
                If Not String.IsNullOrWhiteSpace(status) Then sql.Append("AND m.Status=@Status ") : command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status
                If Not String.IsNullOrWhiteSpace(interventionType) Then sql.Append("AND m.InterventionType=@Type ") : command.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = interventionType
                sql.Append("ORDER BY CASE WHEN m.Status IN (N'Planned',N'InProgress') THEN 0 ELSE 1 END, COALESCE(m.ScheduledAtUtc,m.CreatedAtUtc) DESC;")
                command.CommandText = sql.ToString() : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New MaintenanceListItem With {
                            .MaintenanceInterventionId = reader.GetInt32(reader.GetOrdinal("MaintenanceInterventionId")), .AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")),
                            .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")), .AssetName = reader.GetString(reader.GetOrdinal("AssetName")),
                            .InterventionType = reader.GetString(reader.GetOrdinal("InterventionType")), .Status = reader.GetString(reader.GetOrdinal("Status")),
                            .TechnicianName = ReadNullableString(reader, "TechnicianName"), .ScheduledAtUtc = ReadNullableDate(reader, "ScheduledAtUtc")})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Function GetEligibleAssets() As List(Of LookupOption)
            Return GetOptions("SELECT AssetId, AssetTag + N' - ' + LTRIM(RTRIM(COALESCE(Manufacturer + N' ', N'') + Model)) FROM dbo.Assets WHERE Status NOT IN (N'Retired',N'Lost') ORDER BY AssetTag;")
        End Function

        Public Function GetTechnicians() As List(Of LookupOption)
            Return GetOptions("SELECT u.UserId, u.FirstName + N' ' + u.LastName + N' (' + r.Name + N')' FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId=u.RoleId WHERE u.IsActive=1 AND r.Name IN (N'Administrator',N'ITManager',N'Technician') ORDER BY u.FirstName,u.LastName;")
        End Function

        Public Function CreateIntervention(assetId As Integer, technicianUserId As Integer?, interventionType As String, scheduledAtUtc As DateTime?, diagnosis As String, externalProvider As String) As Integer
            Const sql = "INSERT dbo.MaintenanceInterventions (AssetId,TechnicianUserId,InterventionType,ScheduledAtUtc,Diagnosis,ExternalProvider) OUTPUT INSERTED.MaintenanceInterventionId VALUES (@AssetId,@TechnicianId,@Type,@Scheduled,@Diagnosis,@Provider);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId
                command.Parameters.Add("@TechnicianId", SqlDbType.Int).Value = If(technicianUserId.HasValue, CType(technicianUserId.Value, Object), DBNull.Value)
                command.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = interventionType
                command.Parameters.Add("@Scheduled", SqlDbType.DateTime2).Value = If(scheduledAtUtc.HasValue, CType(scheduledAtUtc.Value, Object), DBNull.Value)
                AddNullableString(command, "@Diagnosis", -1, diagnosis) : AddNullableString(command, "@Provider", 150, externalProvider)
                connection.Open() : Return Convert.ToInt32(command.ExecuteScalar())
            End Using
        End Function

        Public Function GetById(interventionId As Integer) As MaintenanceDetails
            Const sql = "SELECT m.*,a.AssetTag,LTRIM(RTRIM(COALESCE(a.Manufacturer + N' ',N'') + a.Model)) AS AssetName,a.Location,CASE WHEN u.UserId IS NULL THEN NULL ELSE u.FirstName + N' ' + u.LastName END AS TechnicianName FROM dbo.MaintenanceInterventions m INNER JOIN dbo.Assets a ON a.AssetId=m.AssetId LEFT JOIN dbo.Users u ON u.UserId=m.TechnicianUserId WHERE m.MaintenanceInterventionId=@Id;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId : connection.Open()
                Using reader = command.ExecuteReader(CommandBehavior.SingleRow)
                    If Not reader.Read() Then Return Nothing
                    Return New MaintenanceDetails With {
                        .MaintenanceInterventionId = interventionId, .AssetId = reader.GetInt32(reader.GetOrdinal("AssetId")), .TechnicianUserId = ReadNullableInteger(reader, "TechnicianUserId"), .AssetTag = reader.GetString(reader.GetOrdinal("AssetTag")),
                        .AssetName = reader.GetString(reader.GetOrdinal("AssetName")), .AssetLocation = ReadNullableString(reader, "Location"), .InterventionType = reader.GetString(reader.GetOrdinal("InterventionType")),
                        .Status = reader.GetString(reader.GetOrdinal("Status")), .TechnicianName = ReadNullableString(reader, "TechnicianName"), .Diagnosis = ReadNullableString(reader, "Diagnosis"),
                        .WorkPerformed = ReadNullableString(reader, "WorkPerformed"), .ScheduledAtUtc = ReadNullableDate(reader, "ScheduledAtUtc"), .StartedAtUtc = ReadNullableDate(reader, "StartedAtUtc"),
                        .CompletedAtUtc = ReadNullableDate(reader, "CompletedAtUtc"), .LaborCost = ReadNullableDecimal(reader, "LaborCost"), .ExternalProvider = ReadNullableString(reader, "ExternalProvider"),
                        .CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))}
                End Using
            End Using
        End Function

        Public Function GetParts(interventionId As Integer) As List(Of MaintenancePartItem)
            Const sql = "SELECT MaintenancePartId,PartName,PartNumber,Quantity,UnitCost FROM dbo.MaintenanceParts WHERE MaintenanceInterventionId=@Id ORDER BY MaintenancePartId DESC;"
            Dim results As New List(Of MaintenancePartItem)()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId : connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        results.Add(New MaintenancePartItem With {.MaintenancePartId = reader.GetInt32(0), .PartName = reader.GetString(1), .PartNumber = ReadNullableString(reader, "PartNumber"), .Quantity = reader.GetInt32(3), .UnitCost = ReadNullableDecimal(reader, "UnitCost")})
                    End While
                End Using
            End Using
            Return results
        End Function

        Public Sub AddPart(interventionId As Integer, partName As String, partNumber As String, quantity As Integer, unitCost As Decimal?)
            If String.IsNullOrWhiteSpace(partName) Then Throw New InvalidOperationException("Enter the part name.")
            If quantity <= 0 Then Throw New InvalidOperationException("Quantity must be at least one.")
            Const sql = "IF NOT EXISTS(SELECT 1 FROM dbo.MaintenanceInterventions WHERE MaintenanceInterventionId=@Id AND Status IN (N'Planned',N'InProgress')) THROW 51000,'Parts can only be added to an active intervention.',1; INSERT dbo.MaintenanceParts(MaintenanceInterventionId,PartName,PartNumber,Quantity,UnitCost) VALUES(@Id,@Name,@Number,@Quantity,@UnitCost);"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId : command.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = partName.Trim()
                AddNullableString(command, "@Number", 100, partNumber) : command.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity
                Dim cost = command.Parameters.Add("@UnitCost", SqlDbType.Decimal) : cost.Precision = 18 : cost.Scale = 2 : cost.Value = If(unitCost.HasValue, CType(unitCost.Value, Object), DBNull.Value)
                connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub StartIntervention(interventionId As Integer)
            ChangeStatus(interventionId, "InProgress", Nothing, Nothing, Nothing)
        End Sub

        Public Sub UpdateIntervention(interventionId As Integer, technicianUserId As Integer?, interventionType As String, scheduledAtUtc As DateTime?, diagnosis As String, externalProvider As String)
            Const sql = "UPDATE dbo.MaintenanceInterventions SET TechnicianUserId=@Technician,InterventionType=@Type,ScheduledAtUtc=@Scheduled,Diagnosis=@Diagnosis,ExternalProvider=@Provider,UpdatedAtUtc=SYSUTCDATETIME() WHERE MaintenanceInterventionId=@Id AND Status IN(N'Planned',N'InProgress'); IF @@ROWCOUNT=0 THROW 51000,'Only active interventions can be edited.',1;"
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                command.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId : command.Parameters.Add("@Technician", SqlDbType.Int).Value = If(technicianUserId.HasValue, CType(technicianUserId.Value, Object), DBNull.Value) : command.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = interventionType
                command.Parameters.Add("@Scheduled", SqlDbType.DateTime2).Value = If(scheduledAtUtc.HasValue, CType(scheduledAtUtc.Value, Object), DBNull.Value) : AddNullableString(command, "@Diagnosis", -1, diagnosis) : AddNullableString(command, "@Provider", 150, externalProvider)
                connection.Open() : command.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub CompleteIntervention(interventionId As Integer, diagnosis As String, workPerformed As String, laborCost As Decimal?)
            If String.IsNullOrWhiteSpace(workPerformed) Then Throw New InvalidOperationException("Describe the work performed before completing the intervention.")
            ChangeStatus(interventionId, "Completed", diagnosis, workPerformed, laborCost)
        End Sub

        Public Sub CancelIntervention(interventionId As Integer)
            ChangeStatus(interventionId, "Cancelled", Nothing, Nothing, Nothing)
        End Sub

        Private Sub ChangeStatus(interventionId As Integer, targetStatus As String, diagnosis As String, workPerformed As String, laborCost As Decimal?)
            Using connection = Database.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction(IsolationLevel.Serializable)
                    Try
                        Dim assetId As Integer, currentStatus As String
                        Using lookup As New SqlCommand("SELECT AssetId,Status FROM dbo.MaintenanceInterventions WITH (UPDLOCK,HOLDLOCK) WHERE MaintenanceInterventionId=@Id;", connection, transaction)
                            lookup.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId
                            Using reader = lookup.ExecuteReader()
                                If Not reader.Read() Then Throw New InvalidOperationException("The intervention was not found.")
                                assetId = reader.GetInt32(0) : currentStatus = reader.GetString(1)
                            End Using
                        End Using
                        Dim allowed = (currentStatus = "Planned" AndAlso (targetStatus = "InProgress" OrElse targetStatus = "Cancelled")) OrElse (currentStatus = "InProgress" AndAlso (targetStatus = "Completed" OrElse targetStatus = "Cancelled"))
                        If Not allowed Then Throw New InvalidOperationException("This status change is no longer available.")
                        Dim updateSql = "UPDATE dbo.MaintenanceInterventions SET Status=@Status, UpdatedAtUtc=SYSUTCDATETIME(), StartedAtUtc=CASE WHEN @Status=N'InProgress' THEN COALESCE(StartedAtUtc,SYSUTCDATETIME()) ELSE StartedAtUtc END, CompletedAtUtc=CASE WHEN @Status=N'Completed' THEN SYSUTCDATETIME() ELSE CompletedAtUtc END"
                        If targetStatus = "Completed" Then updateSql &= ", Diagnosis=@Diagnosis, WorkPerformed=@Work, LaborCost=@Cost"
                        updateSql &= " WHERE MaintenanceInterventionId=@Id;"
                        Using command As New SqlCommand(updateSql, connection, transaction)
                            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = targetStatus : command.Parameters.Add("@Id", SqlDbType.Int).Value = interventionId
                            If targetStatus = "Completed" Then
                                AddNullableString(command, "@Diagnosis", -1, diagnosis) : AddNullableString(command, "@Work", -1, workPerformed)
                                Dim cost = command.Parameters.Add("@Cost", SqlDbType.Decimal) : cost.Precision = 18 : cost.Scale = 2 : cost.Value = If(laborCost.HasValue, CType(laborCost.Value, Object), DBNull.Value)
                            End If
                            command.ExecuteNonQuery()
                        End Using
                        Dim assetSql = If(targetStatus = "InProgress", "UPDATE dbo.Assets SET Status=N'InMaintenance',UpdatedAtUtc=SYSUTCDATETIME() WHERE AssetId=@AssetId AND Status NOT IN (N'Retired',N'Lost');", "UPDATE dbo.Assets SET Status=CASE WHEN EXISTS(SELECT 1 FROM dbo.AssetAssignments WHERE AssetId=@AssetId AND ReturnedAtUtc IS NULL) THEN N'Assigned' ELSE N'Available' END,UpdatedAtUtc=SYSUTCDATETIME() WHERE AssetId=@AssetId AND Status=N'InMaintenance';")
                        Using assetCommand As New SqlCommand(assetSql, connection, transaction)
                            assetCommand.Parameters.Add("@AssetId", SqlDbType.Int).Value = assetId : assetCommand.ExecuteNonQuery()
                        End Using
                        transaction.Commit()
                    Catch
                        transaction.Rollback() : Throw
                    End Try
                End Using
            End Using
        End Sub

        Private Shared Function GetOptions(sql As String) As List(Of LookupOption)
            Dim results As New List(Of LookupOption)()
            Using connection = Database.CreateConnection(), command As New SqlCommand(sql, connection)
                connection.Open()
                Using reader = command.ExecuteReader()
                    While reader.Read() : results.Add(New LookupOption With {.Id = reader.GetInt32(0), .Label = reader.GetString(1)}) : End While
                End Using
            End Using
            Return results
        End Function
        Private Shared Sub AddNullableString(command As SqlCommand, name As String, length As Integer, value As String)
            Dim parameter = If(length = -1, command.Parameters.Add(name, SqlDbType.NVarChar), command.Parameters.Add(name, SqlDbType.NVarChar, length))
            parameter.Value = If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value.Trim())
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
        Private Shared Function ReadNullableInteger(reader As SqlDataReader, columnName As String) As Integer?
            Dim ordinal = reader.GetOrdinal(columnName) : Return If(reader.IsDBNull(ordinal), CType(Nothing, Integer?), reader.GetInt32(ordinal))
        End Function
    End Class
End Namespace
