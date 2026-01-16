-- 1️⃣ 新增帳號（不要帶 AccountId）
INSERT INTO dbo.Accounts (Username, PasswordHash, Status)
VALUES (N'admin', N'hashed-password', N'Active');

-- 2️⃣ 取得剛剛新增的 AccountId
DECLARE @AccountId UNIQUEIDENTIFIER;

SELECT @AccountId = AccountId
FROM dbo.Accounts
WHERE Username = N'admin';

-- 3️⃣ 指派 Admin 角色
INSERT INTO dbo.AccountRoles(AccountId, RoleId)
SELECT @AccountId, RoleId
FROM dbo.Roles
WHERE RoleCode = N'Admin';