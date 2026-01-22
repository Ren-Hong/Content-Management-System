IF OBJECT_ID('dbo.Permissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permissions
    (
        PermissionId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Permissions PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),

        PermissionCode        NVARCHAR(120) NOT NULL,  -- Content.Publish
        Description NVARCHAR(200) NULL,
        CreatedAt   DATETIME2(0)  NOT NULL CONSTRAINT DF_Permissions_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_Permissions_Code ON dbo.Permissions(PermissionCode);
END

ALTER TABLE dbo.Permissions
ADD Status SMALLINT NOT NULL
    CONSTRAINT DF_Permissions_Status DEFAULT 1;
-- 1 = Enable, 2 = Disabled

ALTER TABLE dbo.Permissions
ADD UpdatedAt DATETIME2(0) NULL;

EXEC sp_rename 
    'Cms_Dev.dbo.Permissions.Description',
    'PermissionName',
    'COLUMN';