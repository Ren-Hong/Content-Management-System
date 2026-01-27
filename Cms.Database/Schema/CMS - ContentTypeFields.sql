CREATE TABLE ContentTypeFields (
    TypeId UNIQUEIDENTIFIER NOT NULL,
    FieldId UNIQUEIDENTIFIER NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0, --欄位顯示排序

    CONSTRAINT PK_ContentTypeFields
        PRIMARY KEY (TypeId, FieldId),

    CONSTRAINT FK_ContentTypeFields_ContentTypes
        FOREIGN KEY (TypeId)
        REFERENCES ContentTypes(TypeId),

    CONSTRAINT FK_ContentTypeFields_ContentFields
        FOREIGN KEY (FieldId)
        REFERENCES ContentFields(FieldId)
);