CREATE TABLE ContentTypes (

    TypeId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ContentTypes PRIMARY KEY
        DEFAULT NEWID(),

    DepartmentId UNIQUEIDENTIFIER NOT NULL,

    TypeCode NVARCHAR(50) NOT NULL,

    TypeName NVARCHAR(100) NOT NULL,

    Description NVARCHAR(255) NULL,

    IsEnabled BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT UQ_ContentTypes_TypeCode UNIQUE (TypeCode),

    CONSTRAINT FK_ContentTypes_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)

);