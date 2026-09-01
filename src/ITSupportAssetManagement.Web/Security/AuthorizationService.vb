Namespace Security
    Public NotInheritable Class AuthorizationService
        Private Sub New()
        End Sub

        Public Shared Function CanAccessPath(roleName As String, appRelativePath As String) As Boolean
            Dim role As String = If(roleName, String.Empty)
            Dim path As String = If(appRelativePath, String.Empty).ToLowerInvariant()
            If role = "Administrator" Then Return True
            If String.IsNullOrWhiteSpace(role) Then Return False

            If path = "~/default.aspx" OrElse path = "~/accessdenied.aspx" OrElse path.StartsWith("~/account/") OrElse path.StartsWith("~/search/") Then Return True
            If path.StartsWith("~/tickets/") Then
                If path.EndsWith("/edit.aspx") Then Return role = "ITManager" OrElse role = "Technician"
                Return True
            End If
            If path.StartsWith("~/assets/") Then
                If path.EndsWith("/create.aspx") OrElse path.EndsWith("/edit.aspx") OrElse path.EndsWith("/assign.aspx") Then Return role = "ITManager"
                Return True
            End If
            If path.StartsWith("~/maintenance/") Then
                If path.EndsWith("/create.aspx") OrElse path.EndsWith("/edit.aspx") Then Return role = "ITManager" OrElse role = "Technician"
                Return True
            End If
            If path = "~/team/list.aspx" OrElse path.StartsWith("~/reports/") Then Return role = "ITManager"
            Return False
        End Function

        Public Shared Function CanManageAssets(roleName As String) As Boolean
            Return roleName = "Administrator" OrElse roleName = "ITManager"
        End Function

        Public Shared Function CanExecuteMaintenance(roleName As String) As Boolean
            Return roleName = "Administrator" OrElse roleName = "ITManager" OrElse roleName = "Technician"
        End Function
    End Class
End Namespace
