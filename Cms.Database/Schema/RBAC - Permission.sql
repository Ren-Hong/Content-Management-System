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

INSERT INTO dbo.Permissions (PermissionCode, PermissionName)
VALUES
(N'Content.View',   N'內容查閱'),
(N'Content.Create', N'內容新增'),
(N'Content.Edit',   N'內容編輯'),
(N'Content.Delete', N'內容刪除');