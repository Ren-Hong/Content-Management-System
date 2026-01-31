CREATE TABLE dbo.RolePermissionScopes
(
    RoleId UNIQUEIDENTIFIER NOT NULL,

    PermissionId UNIQUEIDENTIFIER NOT NULL,

    ScopeId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_RolePermissionScopes_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_RolePermissionScopes
        PRIMARY KEY (RoleId, PermissionId, ScopeId),

    -- 關鍵：Scope 依附 RolePermission
    CONSTRAINT FK_RolePermissionScopes_RolePermissions
        FOREIGN KEY (RoleId, PermissionId)
        REFERENCES dbo.RolePermissions(RoleId, PermissionId)
        ON DELETE CASCADE,

    -- Scope 本身是字典
    CONSTRAINT FK_RolePermissionScopes_Scopes
        FOREIGN KEY (ScopeId)
        REFERENCES dbo.Scopes(ScopeId)
);

INSERT INTO dbo.RolePermissionScopes
(
    RoleId,
    PermissionId,
    ScopeId
)
SELECT
    r.RoleId,
    p.PermissionId,
    s.ScopeId
FROM dbo.Roles r
JOIN dbo.Permissions p
    ON p.PermissionCode LIKE N'Content.%'
JOIN dbo.Scopes s
    ON s.ScopeCode = N'Global'
WHERE r.RoleCode = N'Admin';