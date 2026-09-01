SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs
    (
        AuditLogId    BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLogs PRIMARY KEY,
        ActorUserId   INT NULL,
        ActionName    NVARCHAR(80) NOT NULL,
        EntityType    NVARCHAR(80) NOT NULL,
        EntityKey     NVARCHAR(80) NULL,
        Summary       NVARCHAR(1000) NOT NULL,
        IpAddress     NVARCHAR(64) NULL,
        CreatedAtUtc  DATETIME2(0) NOT NULL CONSTRAINT DF_AuditLogs_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AuditLogs_Actor FOREIGN KEY (ActorUserId) REFERENCES dbo.Users(UserId)
    );
    CREATE INDEX IX_AuditLogs_CreatedAtUtc ON dbo.AuditLogs(CreatedAtUtc DESC);
    CREATE INDEX IX_AuditLogs_Entity ON dbo.AuditLogs(EntityType, EntityKey, CreatedAtUtc DESC);
END;
COMMIT TRANSACTION;
