CREATE TABLE dbo.Departments
(
    DepartmentId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_Departments_DepartmentId
        DEFAULT NEWSEQUENTIALID(),

    DepartmentCode NVARCHAR(50) NOT NULL,   
    DepartmentName NVARCHAR(100) NOT NULL,

    Status SMALLINT NOT NULL
        CONSTRAINT DF_Departments_Status DEFAULT 1,  

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Departments_CreatedAt DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    CONSTRAINT PK_Departments
        PRIMARY KEY (DepartmentId),

    CONSTRAINT UQ_Departments_DepartmentCode
        UNIQUE (DepartmentCode)
);

INSERT INTO dbo.Departments
(
    DepartmentCode,
    DepartmentName
)
VALUES
(N'IT',          N'資訊部');
