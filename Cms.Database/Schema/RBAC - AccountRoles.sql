CREATE TABLE dbo.AccountRoles
(
    AccountId UNIQUEIDENTIFIER NOT NULL,

    RoleId    UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_AccountRoles_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_AccountRoles
        PRIMARY KEY (AccountId, RoleId),

    CONSTRAINT FK_AccountRoles_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES dbo.Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountRoles_Roles
        FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles(RoleId)
        ON DELETE CASCADE
);

INSERT INTO dbo.AccountRoles (AccountId, RoleId)
SELECT
    a.AccountId,
    r.RoleId
FROM dbo.Accounts a
JOIN dbo.Roles r
    ON r.RoleCode = N'Admin'
WHERE a.Username = N'admin';