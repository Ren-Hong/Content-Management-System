CREATE TABLE dbo.RolePermissionScopes
(
    RoleId UNIQUEIDENTIFIER NOT NULL,

    PermissionId UNIQUEIDENTIFIER NOT NULL,

    ScopeId SMALLINT NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_RolePermissionScopes_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_RolePermissionScopes
        PRIMARY KEY (RoleId, PermissionId),

    CONSTRAINT FK_RolePermissionScopes_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles(RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissionScopes_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES dbo.Permissions(PermissionId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissionScopes_Scopes
        FOREIGN KEY (ScopeId)
        REFERENCES dbo.Scopes(ScopeId)
);