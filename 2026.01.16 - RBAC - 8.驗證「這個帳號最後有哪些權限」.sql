SELECT DISTINCT p.PermissionCode
FROM dbo.AccountRoles ar
JOIN dbo.RolePermissions rp ON rp.RoleId = ar.RoleId
JOIN dbo.Permissions p ON p.PermissionId = rp.PermissionId
WHERE ar.AccountId = (
    SELECT AccountId FROM dbo.Accounts WHERE Username = N'admin'
)
ORDER BY p.PermissionCode;