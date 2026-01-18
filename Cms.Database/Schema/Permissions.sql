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

    CREATE UNIQUE INDEX UX_Permissions_Code ON dbo.Permissions(Code);
END

INSERT INTO dbo.Permissions(Code, Description)
VALUES
(N'Content.View',    N'瀏覽內容'),
(N'Content.Create',  N'新增內容'),
(N'Content.Edit',    N'編輯內容'),
(N'Content.Publish', N'發佈內容');