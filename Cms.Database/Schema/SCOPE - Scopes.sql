CREATE TABLE dbo.Scopes
(
    ScopeId SMALLINT NOT NULL,
    ScopeCode NVARCHAR(30) NOT NULL,   -- Global / Department / Self / Assigned
    Description NVARCHAR(100) NULL,

    CONSTRAINT PK_Scopes
        PRIMARY KEY (ScopeId),

    CONSTRAINT UQ_Scopes_ScopeCode
        UNIQUE (ScopeCode)
);

INSERT INTO dbo.Scopes (ScopeId, ScopeCode, Description)
VALUES
(1, N'Global',      N'全系統範圍'),
(2, N'Department',  N'所屬部門'),
(3, N'Self',        N'僅限本人'),
(4, N'Assigned',    N'明確指派');