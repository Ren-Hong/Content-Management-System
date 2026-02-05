CREATE TABLE dbo.DepartmentPermissionAssignments
(
    AccountId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,

    ValidFrom DATETIME2(0) NOT NULL
        CONSTRAINT DF_DPA_ValidFrom DEFAULT SYSUTCDATETIME(),

    ValidTo DATETIME2(0) NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_DPA_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_DepartmentPermissionAssignments
        PRIMARY KEY (AccountId, DepartmentId, PermissionId),

    CONSTRAINT FK_DPA_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES dbo.Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_DPA_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES dbo.Departments(DepartmentId)
        ON DELETE CASCADE,

    CONSTRAINT FK_DPA_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES dbo.Permissions(PermissionId)
        ON DELETE CASCADE
);

CREATE INDEX IX_DPA_Department_Permission
ON dbo.DepartmentPermissionAssignments (DepartmentId, PermissionId);