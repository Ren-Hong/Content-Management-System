CREATE TABLE ContentFieldValues (
    RevisionId UNIQUEIDENTIFIER NOT NULL,

    FieldId UNIQUEIDENTIFIER NOT NULL,

    FieldValue NVARCHAR(MAX) NULL,

    CONSTRAINT PK_ContentFieldValues
        PRIMARY KEY (RevisionId, FieldId),

    CONSTRAINT FK_ContentFieldValues_ContentRevisions
        FOREIGN KEY (RevisionId)
        REFERENCES ContentRevisions(RevisionId),

    CONSTRAINT FK_ContentFieldValues_ContentFields
        FOREIGN KEY (FieldId)
        REFERENCES ContentFields(FieldId)
);