CREATE TABLE ContentRevisions (
    RevisionId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ContentRevisions PRIMARY KEY
        DEFAULT NEWID(),

    ContentId UNIQUEIDENTIFIER NOT NULL,
    Version INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_ContentRevisions_Contents
        FOREIGN KEY (ContentId)
        REFERENCES Contents(ContentId),

    CONSTRAINT UQ_ContentRevisions_Content_Version
        UNIQUE (ContentId, Version)
);