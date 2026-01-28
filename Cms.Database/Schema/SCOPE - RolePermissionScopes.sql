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