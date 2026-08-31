SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Roles
        (
            RoleId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
            Name         NVARCHAR(50) NOT NULL CONSTRAINT UQ_Roles_Name UNIQUE,
            Description  NVARCHAR(250) NULL,
            CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Roles_CreatedAtUtc DEFAULT SYSUTCDATETIME()
        );
    END;

    IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Users
        (
            UserId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
            RoleId       INT NOT NULL,
            EmployeeCode NVARCHAR(30) NULL,
            FirstName    NVARCHAR(80) NOT NULL,
            LastName     NVARCHAR(80) NOT NULL,
            Email        NVARCHAR(254) NOT NULL,
            PasswordHash NVARCHAR(500) NOT NULL,
            Department   NVARCHAR(100) NULL,
            PhoneNumber  NVARCHAR(30) NULL,
            IsActive     BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
            CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
            UpdatedAtUtc DATETIME2(0) NULL,
            CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId),
            CONSTRAINT UQ_Users_Email UNIQUE (Email)
        );
        CREATE UNIQUE INDEX UX_Users_EmployeeCode ON dbo.Users(EmployeeCode) WHERE EmployeeCode IS NOT NULL;
    END;

    IF OBJECT_ID(N'dbo.TicketCategories', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.TicketCategories
        (
            TicketCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TicketCategories PRIMARY KEY,
            Name             NVARCHAR(80) NOT NULL CONSTRAINT UQ_TicketCategories_Name UNIQUE,
            IsActive         BIT NOT NULL CONSTRAINT DF_TicketCategories_IsActive DEFAULT (1)
        );
    END;

    IF OBJECT_ID(N'dbo.AssetCategories', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.AssetCategories
        (
            AssetCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AssetCategories PRIMARY KEY,
            Name            NVARCHAR(80) NOT NULL CONSTRAINT UQ_AssetCategories_Name UNIQUE,
            IsActive        BIT NOT NULL CONSTRAINT DF_AssetCategories_IsActive DEFAULT (1)
        );
    END;

    IF OBJECT_ID(N'dbo.Assets', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Assets
        (
            AssetId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assets PRIMARY KEY,
            AssetCategoryId  INT NOT NULL,
            AssetTag         NVARCHAR(40) NOT NULL,
            SerialNumber     NVARCHAR(100) NULL,
            Manufacturer     NVARCHAR(80) NULL,
            Model            NVARCHAR(120) NOT NULL,
            PurchaseDate     DATE NULL,
            PurchaseCost     DECIMAL(18,2) NULL,
            WarrantyEndDate  DATE NULL,
            Location         NVARCHAR(150) NULL,
            Status           NVARCHAR(30) NOT NULL CONSTRAINT DF_Assets_Status DEFAULT (N'Available'),
            Notes            NVARCHAR(1000) NULL,
            CreatedAtUtc     DATETIME2(0) NOT NULL CONSTRAINT DF_Assets_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
            UpdatedAtUtc     DATETIME2(0) NULL,
            RowVersion       ROWVERSION NOT NULL,
            CONSTRAINT FK_Assets_AssetCategories FOREIGN KEY (AssetCategoryId) REFERENCES dbo.AssetCategories(AssetCategoryId),
            CONSTRAINT UQ_Assets_AssetTag UNIQUE (AssetTag),
            CONSTRAINT CK_Assets_Status CHECK (Status IN (N'Available', N'Assigned', N'InMaintenance', N'Retired', N'Lost')),
            CONSTRAINT CK_Assets_PurchaseCost CHECK (PurchaseCost IS NULL OR PurchaseCost >= 0),
            CONSTRAINT CK_Assets_WarrantyDates CHECK (WarrantyEndDate IS NULL OR PurchaseDate IS NULL OR WarrantyEndDate >= PurchaseDate)
        );
        CREATE UNIQUE INDEX UX_Assets_SerialNumber ON dbo.Assets(SerialNumber) WHERE SerialNumber IS NOT NULL;
    END;

    IF OBJECT_ID(N'dbo.AssetAssignments', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.AssetAssignments
        (
            AssetAssignmentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AssetAssignments PRIMARY KEY,
            AssetId           INT NOT NULL,
            UserId            INT NOT NULL,
            AssignedByUserId  INT NOT NULL,
            AssignedAtUtc     DATETIME2(0) NOT NULL CONSTRAINT DF_AssetAssignments_AssignedAtUtc DEFAULT SYSUTCDATETIME(),
            ReturnedAtUtc     DATETIME2(0) NULL,
            AssignmentNotes   NVARCHAR(500) NULL,
            ReturnNotes       NVARCHAR(500) NULL,
            CONSTRAINT FK_AssetAssignments_Assets FOREIGN KEY (AssetId) REFERENCES dbo.Assets(AssetId),
            CONSTRAINT FK_AssetAssignments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
            CONSTRAINT FK_AssetAssignments_AssignedBy FOREIGN KEY (AssignedByUserId) REFERENCES dbo.Users(UserId),
            CONSTRAINT CK_AssetAssignments_ReturnDate CHECK (ReturnedAtUtc IS NULL OR ReturnedAtUtc >= AssignedAtUtc)
        );
        CREATE UNIQUE INDEX UX_AssetAssignments_ActiveAsset ON dbo.AssetAssignments(AssetId) WHERE ReturnedAtUtc IS NULL;
        CREATE INDEX IX_AssetAssignments_UserId ON dbo.AssetAssignments(UserId, ReturnedAtUtc);
    END;

    IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Tickets
        (
            TicketId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tickets PRIMARY KEY,
            TicketNumber      AS (N'TKT-' + RIGHT(N'000000' + CONVERT(NVARCHAR(10), TicketId), 6)) PERSISTED,
            TicketCategoryId  INT NOT NULL,
            AssetId           INT NULL,
            RequestedByUserId INT NOT NULL,
            AssignedToUserId  INT NULL,
            Title             NVARCHAR(180) NOT NULL,
            Description       NVARCHAR(MAX) NOT NULL,
            Priority          NVARCHAR(20) NOT NULL CONSTRAINT DF_Tickets_Priority DEFAULT (N'Medium'),
            Status            NVARCHAR(30) NOT NULL CONSTRAINT DF_Tickets_Status DEFAULT (N'Open'),
            Resolution        NVARCHAR(MAX) NULL,
            DueAtUtc          DATETIME2(0) NULL,
            ResolvedAtUtc     DATETIME2(0) NULL,
            ClosedAtUtc       DATETIME2(0) NULL,
            CreatedAtUtc      DATETIME2(0) NOT NULL CONSTRAINT DF_Tickets_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
            UpdatedAtUtc      DATETIME2(0) NULL,
            RowVersion        ROWVERSION NOT NULL,
            CONSTRAINT FK_Tickets_TicketCategories FOREIGN KEY (TicketCategoryId) REFERENCES dbo.TicketCategories(TicketCategoryId),
            CONSTRAINT FK_Tickets_Assets FOREIGN KEY (AssetId) REFERENCES dbo.Assets(AssetId),
            CONSTRAINT FK_Tickets_RequestedBy FOREIGN KEY (RequestedByUserId) REFERENCES dbo.Users(UserId),
            CONSTRAINT FK_Tickets_AssignedTo FOREIGN KEY (AssignedToUserId) REFERENCES dbo.Users(UserId),
            CONSTRAINT UQ_Tickets_TicketNumber UNIQUE (TicketNumber),
            CONSTRAINT CK_Tickets_Priority CHECK (Priority IN (N'Low', N'Medium', N'High', N'Critical')),
            CONSTRAINT CK_Tickets_Status CHECK (Status IN (N'Open', N'Assigned', N'InProgress', N'Waiting', N'Resolved', N'Closed', N'Cancelled')),
            CONSTRAINT CK_Tickets_ResolvedDate CHECK (ResolvedAtUtc IS NULL OR ResolvedAtUtc >= CreatedAtUtc),
            CONSTRAINT CK_Tickets_ClosedDate CHECK (ClosedAtUtc IS NULL OR ResolvedAtUtc IS NULL OR ClosedAtUtc >= ResolvedAtUtc)
        );
        CREATE INDEX IX_Tickets_StatusPriority ON dbo.Tickets(Status, Priority, CreatedAtUtc DESC);
        CREATE INDEX IX_Tickets_AssignedTo ON dbo.Tickets(AssignedToUserId, Status);
        CREATE INDEX IX_Tickets_RequestedBy ON dbo.Tickets(RequestedByUserId, CreatedAtUtc DESC);
        CREATE INDEX IX_Tickets_AssetId ON dbo.Tickets(AssetId) WHERE AssetId IS NOT NULL;
    END;

    IF OBJECT_ID(N'dbo.TicketComments', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.TicketComments
        (
            TicketCommentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TicketComments PRIMARY KEY,
            TicketId        INT NOT NULL,
            AuthorUserId    INT NOT NULL,
            Body            NVARCHAR(MAX) NOT NULL,
            IsInternal      BIT NOT NULL CONSTRAINT DF_TicketComments_IsInternal DEFAULT (0),
            CreatedAtUtc    DATETIME2(0) NOT NULL CONSTRAINT DF_TicketComments_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
            CONSTRAINT FK_TicketComments_Tickets FOREIGN KEY (TicketId) REFERENCES dbo.Tickets(TicketId),
            CONSTRAINT FK_TicketComments_Users FOREIGN KEY (AuthorUserId) REFERENCES dbo.Users(UserId)
        );
        CREATE INDEX IX_TicketComments_TicketId ON dbo.TicketComments(TicketId, CreatedAtUtc);
    END;

    IF OBJECT_ID(N'dbo.MaintenanceInterventions', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.MaintenanceInterventions
        (
            MaintenanceInterventionId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MaintenanceInterventions PRIMARY KEY,
            AssetId                  INT NOT NULL,
            TicketId                 INT NULL,
            TechnicianUserId         INT NULL,
            InterventionType         NVARCHAR(30) NOT NULL,
            Status                   NVARCHAR(30) NOT NULL CONSTRAINT DF_Maintenance_Status DEFAULT (N'Planned'),
            Diagnosis                NVARCHAR(MAX) NULL,
            WorkPerformed            NVARCHAR(MAX) NULL,
            ScheduledAtUtc           DATETIME2(0) NULL,
            StartedAtUtc             DATETIME2(0) NULL,
            CompletedAtUtc           DATETIME2(0) NULL,
            LaborCost                DECIMAL(18,2) NULL,
            ExternalProvider         NVARCHAR(150) NULL,
            CreatedAtUtc             DATETIME2(0) NOT NULL CONSTRAINT DF_Maintenance_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
            UpdatedAtUtc             DATETIME2(0) NULL,
            RowVersion               ROWVERSION NOT NULL,
            CONSTRAINT FK_Maintenance_Assets FOREIGN KEY (AssetId) REFERENCES dbo.Assets(AssetId),
            CONSTRAINT FK_Maintenance_Tickets FOREIGN KEY (TicketId) REFERENCES dbo.Tickets(TicketId),
            CONSTRAINT FK_Maintenance_Technicians FOREIGN KEY (TechnicianUserId) REFERENCES dbo.Users(UserId),
            CONSTRAINT CK_Maintenance_Type CHECK (InterventionType IN (N'Preventive', N'Corrective', N'Inspection', N'Upgrade')),
            CONSTRAINT CK_Maintenance_Status CHECK (Status IN (N'Planned', N'InProgress', N'Completed', N'Cancelled')),
            CONSTRAINT CK_Maintenance_LaborCost CHECK (LaborCost IS NULL OR LaborCost >= 0),
            CONSTRAINT CK_Maintenance_Dates CHECK (CompletedAtUtc IS NULL OR StartedAtUtc IS NULL OR CompletedAtUtc >= StartedAtUtc)
        );
        CREATE INDEX IX_Maintenance_AssetId ON dbo.MaintenanceInterventions(AssetId, CreatedAtUtc DESC);
        CREATE INDEX IX_Maintenance_TechnicianStatus ON dbo.MaintenanceInterventions(TechnicianUserId, Status);
    END;

    IF OBJECT_ID(N'dbo.MaintenanceParts', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.MaintenanceParts
        (
            MaintenancePartId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MaintenanceParts PRIMARY KEY,
            MaintenanceInterventionId INT NOT NULL,
            PartName                  NVARCHAR(150) NOT NULL,
            PartNumber                NVARCHAR(100) NULL,
            Quantity                  INT NOT NULL CONSTRAINT DF_MaintenanceParts_Quantity DEFAULT (1),
            UnitCost                  DECIMAL(18,2) NULL,
            CONSTRAINT FK_MaintenanceParts_Interventions FOREIGN KEY (MaintenanceInterventionId) REFERENCES dbo.MaintenanceInterventions(MaintenanceInterventionId),
            CONSTRAINT CK_MaintenanceParts_Quantity CHECK (Quantity > 0),
            CONSTRAINT CK_MaintenanceParts_UnitCost CHECK (UnitCost IS NULL OR UnitCost >= 0)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
    BEGIN
        INSERT dbo.Roles (Name, Description)
        VALUES (N'Administrator', N'Full platform administration'),
               (N'ITManager', N'IT operations and reporting'),
               (N'Technician', N'Ticket and maintenance execution'),
               (N'Employee', N'Support requester and assigned asset user');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories)
    BEGIN
        INSERT dbo.TicketCategories (Name)
        VALUES (N'Hardware'), (N'Software'), (N'Network'), (N'Access'), (N'Email'), (N'Other');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.AssetCategories)
    BEGIN
        INSERT dbo.AssetCategories (Name)
        VALUES (N'Laptop'), (N'Desktop'), (N'Monitor'), (N'Printer'), (N'Server'), (N'Network Equipment'), (N'Mobile Device'), (N'Peripheral');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
