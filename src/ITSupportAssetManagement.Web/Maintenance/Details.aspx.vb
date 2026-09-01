Imports System.Data.SqlClient
Imports System.Globalization
Imports ITSupportAssetManagement.Web.Data
Public Partial Class MaintenanceDetailsPage
    Inherits System.Web.UI.Page
    Private ReadOnly _maintenance As New MaintenanceRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then LoadDetails()
    End Sub
    Private Sub LoadDetails()
        Dim interventionId As Integer
        If Not Integer.TryParse(Request.QueryString("id"), interventionId) Then ShowNotFound() : Return
        Try
            Dim item = _maintenance.GetById(interventionId)
            If item Is Nothing Then ShowNotFound() : Return
            InterventionNumber.Text = interventionId.ToString("D4") : InterventionType.Text = Server.HtmlEncode(item.InterventionType) : DetailType.Text = InterventionType.Text
            HeadingAsset.Text = Server.HtmlEncode(item.AssetTag) : AssetName.Text = Server.HtmlEncode(item.AssetName) : AssetTag.Text = Server.HtmlEncode(item.AssetTag) : AssetLink.HRef = "../Assets/Details.aspx?id=" & item.AssetId.ToString()
            AssetLocation.Text = Display(item.AssetLocation, "Not specified") : Technician.Text = Display(item.TechnicianName, "Unassigned") : Provider.Text = Display(item.ExternalProvider, "Internal service")
            Diagnosis.Text = Display(item.Diagnosis, "No diagnosis has been recorded yet.") : WorkPerformed.Text = Display(item.WorkPerformed, "Work details will appear when the intervention is completed.")
            Scheduled.Text = DisplayDate(item.ScheduledAtUtc) : Started.Text = DisplayDate(item.StartedAtUtc) : Completed.Text = DisplayDate(item.CompletedAtUtc) : LaborCost.Text = If(item.LaborCost.HasValue, item.LaborCost.Value.ToString("N2") & " TND", "Not recorded")
            Dim parts = _maintenance.GetParts(interventionId), partsCost = parts.Sum(Function(part) part.LineTotal)
            PartsRepeater.DataSource = parts : PartsRepeater.DataBind() : NoPartsPanel.Visible = parts.Count = 0
            PartsTotal.Text = partsCost.ToString("N2") & " TND" : TotalCost.Text = (partsCost + If(item.LaborCost.HasValue, item.LaborCost.Value, 0D)).ToString("N2") & " TND"
            StatusBadge.Text = item.Status.Replace("InProgress", "In progress") : StatusBadge.CssClass = "maintenance-state maintenance-" & item.Status.ToLowerInvariant()
            Dim role = Convert.ToString(Session("RoleName")), canExecute = role = "Administrator" OrElse role = "ITManager" OrElse role = "Technician"
            StartButton.Visible = canExecute AndAlso item.Status = "Planned" : CompletionPanel.Visible = canExecute AndAlso item.Status = "InProgress" : CancelButton.Visible = canExecute AndAlso (item.Status = "Planned" OrElse item.Status = "InProgress") : ActionsPanel.Visible = StartButton.Visible OrElse CancelButton.Visible
            AddPartPanel.Visible = canExecute AndAlso (item.Status = "Planned" OrElse item.Status = "InProgress")
            If CompletionPanel.Visible Then DiagnosisInput.Text = item.Diagnosis
            If Request.QueryString("created") = "1" Then SuccessMessage.Text = "The intervention was planned successfully." : SuccessPanel.Visible = True
            If Request.QueryString("updated") = "1" Then SuccessMessage.Text = "The intervention workflow was updated successfully." : SuccessPanel.Visible = True
            If Request.QueryString("partAdded") = "1" Then SuccessMessage.Text = "The part was added to the intervention cost." : SuccessPanel.Visible = True
            DetailsPanel.Visible = True
        Catch ex As SqlException
            ShowNotFound()
        End Try
    End Sub
    Protected Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        ExecuteAction(Sub() _maintenance.StartIntervention(GetId()))
    End Sub
    Protected Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        ExecuteAction(Sub() _maintenance.CancelIntervention(GetId()))
    End Sub
    Protected Sub CompleteButton_Click(sender As Object, e As EventArgs) Handles CompleteButton.Click
        If Not Page.IsValid Then Return
        Dim cost As Decimal? = Nothing, parsedCost As Decimal
        If Not String.IsNullOrWhiteSpace(LaborCostInput.Text) Then
            If Not Decimal.TryParse(LaborCostInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, parsedCost) OrElse parsedCost < 0 Then ShowError("Enter a valid non-negative labor cost.") : Return
            cost = parsedCost
        End If
        ExecuteAction(Sub() _maintenance.CompleteIntervention(GetId(), DiagnosisInput.Text, WorkInput.Text, cost))
    End Sub
    Protected Sub AddPartButton_Click(sender As Object, e As EventArgs) Handles AddPartButton.Click
        Dim quantity As Integer, cost As Decimal? = Nothing, parsedCost As Decimal
        If Not Integer.TryParse(QuantityInput.Text, quantity) OrElse quantity <= 0 Then ShowError("Enter a valid quantity.") : Return
        If Not String.IsNullOrWhiteSpace(UnitCostInput.Text) Then
            If Not Decimal.TryParse(UnitCostInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, parsedCost) OrElse parsedCost < 0 Then ShowError("Enter a valid non-negative unit cost.") : Return
            cost = parsedCost
        End If
        Try
            _maintenance.AddPart(GetId(), PartNameInput.Text, PartNumberInput.Text, quantity, cost)
            Response.Redirect("~/Maintenance/Details.aspx?id=" & GetId().ToString() & "&partAdded=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The part could not be added. Refresh and try again.")
        End Try
    End Sub
    Protected Function DisplayPartNumber(value As Object) As String
        Dim text = Convert.ToString(value) : Return If(String.IsNullOrWhiteSpace(text), "No part number", text)
    End Function
    Protected Shared Function DisplayMoney(value As Object) As String
        If value Is Nothing OrElse value Is DBNull.Value Then Return "0.00 TND"
        Return Convert.ToDecimal(value).ToString("N2") & " TND"
    End Function
    Private Sub ExecuteAction(action As Action)
        Try
            action() : Response.Redirect("~/Maintenance/Details.aspx?id=" & GetId().ToString() & "&updated=1", False)
        Catch ex As InvalidOperationException
            ShowError(ex.Message)
        Catch ex As SqlException
            ShowError("The workflow could not be updated. Refresh and try again.")
        End Try
    End Sub
    Private Function GetId() As Integer
        Dim id As Integer : Integer.TryParse(Request.QueryString("id"), id) : Return id
    End Function
    Private Function Display(value As String, fallback As String) As String
        Return Server.HtmlEncode(If(String.IsNullOrWhiteSpace(value), fallback, value))
    End Function
    Private Shared Function DisplayDate(value As DateTime?) As String
        Return If(value.HasValue, value.Value.ToLocalTime().ToString("dd MMM yyyy, HH:mm"), "Not recorded")
    End Function
    Private Sub ShowError(message As String)
        ErrorMessage.Text = Server.HtmlEncode(message) : ErrorPanel.Visible = True
    End Sub
    Private Sub ShowNotFound()
        NotFoundPanel.Visible = True : DetailsPanel.Visible = False
    End Sub
End Class
