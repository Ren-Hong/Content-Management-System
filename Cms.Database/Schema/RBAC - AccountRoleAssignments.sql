CREATE TABLE dbo.AccountRoleAssignments
(
    AccountId UNIQUEIDENTIFIER NOT NULL,

    RoleId    UNIQUEIDENTIFIER NOT NULL,

    DepartmentId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_AccountRoles_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_AccountRolesAssignments
        PRIMARY KEY (AccountId, RoleId),

    CONSTRAINT FK_AccountRolesAssignments_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES dbo.Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountRolesAssignments_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles(RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountRolesAssignments_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES dbo.Departments(DepartmentId)
        ON DELETE CASCADE
);

INSERT INTO dbo.AccountRoleAssignments (AccountId, RoleId, DepartmentId)
SELECT
    a.AccountId,
    r.RoleId,
    d.DepartmentId
FROM dbo.Accounts a
JOIN dbo.Roles r ON r.RoleCode = N'Admin'
JOIN dbo.Departments d ON d.DepartmentCode = N'IT'
WHERE a.Username = N'admin';