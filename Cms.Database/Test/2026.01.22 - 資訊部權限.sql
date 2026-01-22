INSERT INTO dbo.Permissions
(
    PermissionCode,
    PermissionName,
    CreatedAt,
    Status
)
VALUES
('IT.View',    N'資訊部查閱', SYSDATETIME(), 1),
('IT.Edit',    N'資訊部編輯', SYSDATETIME(), 1),
('IT.Create',  N'資訊部新增', SYSDATETIME(), 1),
('IT.Delete',  N'資訊部刪除', SYSDATETIME(), 1)