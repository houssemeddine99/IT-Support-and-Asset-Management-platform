SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.PasswordResetTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetTokens
    (
        PasswordResetTokenId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PasswordResetTokens PRIMARY KEY,
        UserId INT NOT NULL,
        TokenHash BINARY(32) NOT NULL,
        ExpiresAtUtc DATETIME2(0) NOT NULL,
        CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_PasswordResetTokens_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UsedAtUtc DATETIME2(0) NULL,
        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY(UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT UQ_PasswordResetTokens_TokenHash UNIQUE(TokenHash)
    );
    CREATE INDEX IX_PasswordResetTokens_User_Active ON dbo.PasswordResetTokens(UserId, UsedAtUtc, ExpiresAtUtc);
END;
COMMIT TRANSACTION;
