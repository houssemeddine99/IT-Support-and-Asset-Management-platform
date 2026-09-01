Namespace Models
    Public NotInheritable Class TicketAttachmentItem
        Public Property TicketAttachmentId As Integer
        Public Property FileName As String
        Public Property ContentType As String
        Public Property FileSizeBytes As Integer
        Public Property UploadedByName As String
        Public Property CreatedAtUtc As DateTime
        Public Property FileContent As Byte()
        Public ReadOnly Property FileSizeLabel As String
            Get
                If FileSizeBytes >= 1048576 Then Return (FileSizeBytes / 1048576.0R).ToString("0.0") & " MB"
                Return Math.Max(1, CInt(Math.Ceiling(FileSizeBytes / 1024.0R))).ToString() & " KB"
            End Get
        End Property
    End Class
End Namespace
