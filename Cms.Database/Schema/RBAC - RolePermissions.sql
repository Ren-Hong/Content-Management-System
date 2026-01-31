CREATE TABLE dbo.RolePermissions
(
    RoleId UNIQUEIDENTIFIER NOT NULL,

    PermissionId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_RolePermissions_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_RolePermissions
        PRIMARY KEY (RoleId, PermissionId),

    CONSTRAINT FK_RolePermissions_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles(RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissions_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES dbo.Permissions(PermissionId)
        ON DELETE CASCADE
);

INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT
    r.RoleId,
    p.PermissionId
FROM dbo.Roles r
JOIN dbo.Permissions p
    ON p.PermissionCode LIKE N'Content.%'
WHERE r.RoleCode = N'Admin';