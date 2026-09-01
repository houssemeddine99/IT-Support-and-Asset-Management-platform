SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.TicketAttachments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TicketAttachments
    (
        TicketAttachmentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TicketAttachments PRIMARY KEY,
        TicketId           INT NOT NULL,
        UploadedByUserId   INT NOT NULL,
        FileName           NVARCHAR(255) NOT NULL,
        ContentType        NVARCHAR(120) NOT NULL,
        FileSizeBytes      INT NOT NULL,
        FileContent        VARBINARY(MAX) NOT NULL,
        CreatedAtUtc       DATETIME2(0) NOT NULL CONSTRAINT DF_TicketAttachments_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TicketAttachments_Tickets FOREIGN KEY (TicketId) REFERENCES dbo.Tickets(TicketId),
        CONSTRAINT FK_TicketAttachments_Users FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT CK_TicketAttachments_FileSize CHECK (FileSizeBytes > 0 AND FileSizeBytes <= 5242880)
    );
    CREATE INDEX IX_TicketAttachments_TicketId ON dbo.TicketAttachments(TicketId, CreatedAtUtc DESC);
END;

COMMIT TRANSACTION;
