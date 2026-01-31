CREATE TABLE dbo.Roles
(
    RoleId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_Roles_RoleId
        DEFAULT NEWSEQUENTIALID(),

    RoleCode NVARCHAR(50) NOT NULL,    -- Admin / Editor / Reviewer
    RoleName NVARCHAR(100) NOT NULL,   -- 系統管理員 / 內容編輯 / 內容審核

    Status SMALLINT NOT NULL
        CONSTRAINT DF_Roles_Status DEFAULT 1,  -- 1=Enable, 2=Disabled

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Roles_CreatedAt DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    CONSTRAINT PK_Roles
        PRIMARY KEY (RoleId),

    CONSTRAINT UQ_Roles_RoleCode
        UNIQUE (RoleCode)
);

INSERT INTO dbo.Roles (RoleCode, RoleName)
VALUES
(N'Admin', N'系統管理員');