Imports System.Data.SqlClient
Imports ITSupportAssetManagement.Web.Data
Public Partial Class MaintenanceListPage
    Inherits System.Web.UI.Page
    Private ReadOnly _maintenance As New MaintenanceRepository()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Request.QueryString("created") = "1" Then SuccessPanel.Visible = True
            BindInterventions()
        End If
    End Sub
    Protected Sub FilterButton_Click(sender As Object, e As EventArgs) Handles FilterButton.Click
        BindInterventions()
    End Sub
    Private Sub BindInterventions()
        Try
            Dim rows = _maintenance.GetInterventions(SearchInput.Text, StatusFilter.SelectedValue, TypeFilter.SelectedValue)
            InterventionRepeater.DataSource = rows : InterventionRepeater.DataBind() : ResultCount.Text = rows.Count.ToString() : EmptyPanel.Visible = rows.Count = 0 : ErrorPanel.Visible = False
        Catch ex As SqlException
            ErrorMessage.Text = "Maintenance interventions could not be loaded. Verify the database connection." : ErrorPanel.Visible = True
        End Try
    End Sub
    Protected Function DisplayOrDefault(value As Object, fallback As String) As String
        Dim text = Convert.ToString(value) : Return If(String.IsNullOrWhiteSpace(text), fallback, text)
    End Function
    Protected Function DisplaySchedule(value As Object) As String
        If value Is Nothing OrElse value Is DBNull.Value Then Return "Not scheduled"
        Return DirectCast(value, DateTime).ToLocalTime().ToString("dd MMM yyyy, HH:mm")
    End Function
End Class
