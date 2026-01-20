IF OBJECT_ID('dbo.Accounts', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounts
    (
        AccountId     UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Accounts PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),

        Username      NVARCHAR(50)  NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,

        Status        SMALLINT  NOT NULL, 
        CreatedAt     DATETIME2(0)  NOT NULL CONSTRAINT DF_Accounts_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt     DATETIME2(0)  NULL,
        LastLoginAt   DATETIME2(0)  NULL
    );

    CREATE UNIQUE INDEX UX_Accounts_Username ON dbo.Accounts(Username);
END