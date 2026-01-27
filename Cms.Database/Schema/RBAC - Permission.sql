CREATE TABLE dbo.Permissions
(
    PermissionId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_Permissions_PermissionId
        DEFAULT NEWSEQUENTIALID(),

    PermissionCode NVARCHAR(120) NOT NULL,   -- e.g. Content.Publish

    PermissionName NVARCHAR(200) NULL, 

    Status SMALLINT NOT NULL
        CONSTRAINT DF_Permissions_Status DEFAULT 1,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Permissions_CreatedAt DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    CONSTRAINT PK_Permissions
        PRIMARY KEY (PermissionId),

    CONSTRAINT UQ_Permissions_PermissionCode
        UNIQUE (PermissionCode)
);