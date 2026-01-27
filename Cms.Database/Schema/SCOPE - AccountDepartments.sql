CREATE TABLE dbo.AccountDepartments
(
    AccountId UNIQUEIDENTIFIER NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,

    IsPrimary BIT NOT NULL
        CONSTRAINT DF_AccountDepartments_IsPrimary DEFAULT 0,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_AccountDepartments_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_AccountDepartments
        PRIMARY KEY (AccountId, DepartmentId),

    CONSTRAINT FK_AccountDepartments_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES dbo.Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountDepartments_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES dbo.Departments(DepartmentId)
        ON DELETE CASCADE
);