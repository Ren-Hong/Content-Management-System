IF OBJECT_ID('dbo.RolePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolePermissions
    (
        RoleId       UNIQUEIDENTIFIER NOT NULL,
        PermissionId UNIQUEIDENTIFIER NOT NULL,
        CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_RolePermissions_CreatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId)
    );

    ALTER TABLE dbo.RolePermissions
      ADD CONSTRAINT FK_RolePermissions_Roles
      FOREIGN KEY (RoleId)
      REFERENCES dbo.Roles(RoleId)
      ON DELETE CASCADE;

    ALTER TABLE dbo.RolePermissions
      ADD CONSTRAINT FK_RolePermissions_Permissions
      FOREIGN KEY (PermissionId)
      REFERENCES dbo.Permissions(PermissionId)
      ON DELETE CASCADE;
END