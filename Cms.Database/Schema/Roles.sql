IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        RoleId    UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Roles PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),

        RoleCode  NVARCHAR(50)  NOT NULL,   -- Admin / Editor
        RoleName  NVARCHAR(100) NOT NULL,   -- 系統管理員 / 內容編輯
        CreatedAt DATETIME2(0)  NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_Roles_RoleCode ON dbo.Roles(RoleCode);
END

INSERT INTO dbo.Roles(RoleCode, RoleName)
VALUES
(N'Admin',    N'系統管理員'),
(N'Editor',   N'內容編輯'),
(N'Reviewer', N'內容審核');

ALTER TABLE dbo.Roles
ADD Status SMALLINT NOT NULL
    CONSTRAINT DF_Roles_Status DEFAULT 1;
-- 1 = Enable, 2 = Disabled

ALTER TABLE dbo.Roles
ADD UpdatedAt DATETIME2(0) NULL;