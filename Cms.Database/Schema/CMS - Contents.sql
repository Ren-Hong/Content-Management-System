CREATE TABLE Contents (
    ContentId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Contents PRIMARY KEY
        DEFAULT NEWID(),

    TypeId UNIQUEIDENTIFIER NOT NULL,

    OwnerId UNIQUEIDENTIFIER NOT NULL, -- Accounts.AccountId

    Status NVARCHAR(30) NOT NULL,     -- Draft / Review / Published / Archived

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Contents_ContentTypes
        FOREIGN KEY (TypeId)
        REFERENCES ContentTypes(TypeId),

    CONSTRAINT FK_Contents_Owner
        FOREIGN KEY (OwnerId)
        REFERENCES Accounts(AccountId)
);