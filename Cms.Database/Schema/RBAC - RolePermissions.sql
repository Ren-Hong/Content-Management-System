CREATE TABLE dbo.RolePermissions
(
    RoleId       UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    ScopeId      UNIQUEIDENTIFIER NOT NULL,

    CreatedAt    DATETIME2(0) NOT NULL
        CONSTRAINT DF_RolePermissions_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_RolePermissions
        PRIMARY KEY (RoleId, PermissionId, ScopeId),

    CONSTRAINT FK_RolePermissions_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles (RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissions_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES dbo.Permissions (PermissionId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissions_Scopes
        FOREIGN KEY (ScopeId)
        REFERENCES dbo.Scopes (ScopeId)
);

INSERT INTO dbo.RolePermissions (RoleId, PermissionId, ScopeId)
SELECT
    r.RoleId,
    p.PermissionId,
    s.ScopeId
FROM dbo.Roles r
JOIN dbo.Permissions p
    ON p.PermissionCode LIKE N'Content.%'
JOIN dbo.Scopes s
    ON s.ScopeCode = N'Department'
WHERE r.RoleCode = N'Admin';