CREATE TABLE dbo.Accounts
(
    AccountId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_Accounts_AccountId DEFAULT NEWSEQUENTIALID(),

    Username NVARCHAR(50) NOT NULL,

    PasswordHash NVARCHAR(255) NOT NULL,

    Status SMALLINT NOT NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Accounts_CreatedAt DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    LastLoginAt DATETIME2(0) NULL,

    CONSTRAINT PK_Accounts
        PRIMARY KEY (AccountId),

    CONSTRAINT UQ_Accounts_Username
        UNIQUE (Username)
);
