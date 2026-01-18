-- Admin = 全部權限
DECLARE @AdminRoleId UNIQUEIDENTIFIER =
(SELECT RoleId FROM dbo.Roles WHERE RoleCode = N'Admin');

INSERT INTO dbo.RolePermissions(RoleId, PermissionId)
SELECT @AdminRoleId, p.PermissionId
FROM dbo.Permissions p;

-- Editor = View / Create / Edit
DECLARE @EditorRoleId UNIQUEIDENTIFIER =
(SELECT RoleId FROM dbo.Roles WHERE RoleCode = N'Editor');

INSERT INTO dbo.RolePermissions(RoleId, PermissionId)
SELECT @EditorRoleId, p.PermissionId
FROM dbo.Permissions p
WHERE p.PermissionCode IN (N'Content.View', N'Content.Create', N'Content.Edit');