CREATE TABLE dbo.Scopes
(
    ScopeId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_Scopes_ScopeId
        DEFAULT NEWSEQUENTIALID(),

    ScopeCode NVARCHAR(30) NOT NULL,   -- Global / Department / Self / Assigned

    ScopeName NVARCHAR(100) NULL,

    CONSTRAINT PK_Scopes
        PRIMARY KEY (ScopeId),

    CONSTRAINT UQ_Scopes_ScopeCode
        UNIQUE (ScopeCode)
);

INSERT INTO dbo.Scopes (ScopeCode, ScopeName)
VALUES
(N'Global',      N'全系統範圍'),
(N'Department',  N'所屬部門'),
(N'Self',        N'僅限本人'),
(N'Assigned',    N'明確指派');