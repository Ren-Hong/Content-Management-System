CREATE TABLE ContentFields (
    FieldId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_ContentFields_FieldId DEFAULT NEWID(),

    FieldCode NVARCHAR(50) NOT NULL,

    FieldType NVARCHAR(30) NOT NULL,   -- text / richtext / number / enum

    IsRequired BIT NOT NULL DEFAULT 0, -- 該表單欄位是否為必填

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT PK_ContentFields
        PRIMARY KEY (FieldId),

    CONSTRAINT UQ_ContentFields_FieldCode
        UNIQUE (FieldCode)
);