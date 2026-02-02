using Cms.Contract.Repositories.Account.Entities;
using Cms.Contract.Repositories.Account.Interfaces;
using Cms.Contract.Repositories.Account.Persistence;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Account
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ): base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<AccountAuthEntity>> GetAccountAuthByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT
                    a.AccountId,
                    a.Username,
                    a.PasswordHash,
                    r.RoleCode,
                    p.PermissionCode
                FROM Accounts a
                LEFT JOIN AccountRoles ar 
                    ON a.AccountId = ar.AccountId
                LEFT JOIN Roles r 
                    ON ar.RoleId = r.RoleId
                    AND r.Status = 1
                LEFT JOIN RolePermissions rp 
                    ON r.RoleId = rp.RoleId
                LEFT JOIN Permissions p 
                    ON rp.PermissionId = p.PermissionId
                    AND p.Status = 1
                WHERE a.Username = @Username
                    AND a.Status = 1;
            ";

            return await _db.QueryAsync<AccountAuthEntity>(
                sql,
                new { Username = username }
            );
        }

        public async Task<IEnumerable<AccountSummaryEntity>> GetAccountSummariesAsync()
        {
            const string sql = @"
                SELECT
                    a.AccountId,
                    a.Username,
                    r.RoleId,
                    r.RoleName,
                    a.Status
                FROM Accounts a
                LEFT JOIN AccountRoles ar 
                    ON a.AccountId = ar.AccountId
                LEFT JOIN Roles r 
                    ON ar.RoleId = r.RoleId
                ORDER BY a.Username;
            ";

            return await _db.QueryAsync<AccountSummaryEntity>(sql);
        }

        public async Task<bool> AccountExistsAsync(string username)
        {
            const string sql = @"
                SELECT 1
                FROM Accounts
                WHERE Username = @Username
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { Username = username }
            );

            return result != null; // 1 -> true, null -> false
        }

        public async Task<Guid> CreateAccountAsync(string username, string passwordHash)
        {
            const string sql = @"
                INSERT INTO Accounts
                (
                    AccountId,
                    Username,
                    PasswordHash,
                    Status,
                    CreatedAt
                )
                OUTPUT INSERTED.AccountId
                VALUES
                (
                    NEWID(),
                    @Username,
                    @PasswordHash,
                    1,
                    GETUTCDATE()
                );
            ";

            var accountId = await _db.ExecuteScalarAsync<Guid>(
                sql, 
                new
                {
                    Username = username,
                    PasswordHash = passwordHash
                },
                transaction: Tx   // 🔥 關鍵 有交易Tx就不會是null, service層會先new起來
            );

            return accountId;
        }

        public async Task CreateAccountRoleAsync(Guid accountId, Guid roleId)
        {
            const string sql = @"
                INSERT INTO AccountRoles
                (
                    AccountId,
                    RoleId
                )
                VALUES
                (
                    @AccountId,
                    @RoleId
                );
            ";

            await _db.ExecuteAsync(
                sql, 
                new
                {
                    AccountId = accountId,
                    RoleId = roleId
                },
                transaction: Tx  // 🔥 一樣要
            );
        }

        public async Task UpdateLastLoginAtAsync(Guid accountId, DateTime loginTime)
        {
            const string sql = @"
                UPDATE Accounts
                SET LastLoginAt = @LastLoginAt
                WHERE AccountId = @AccountId
            ";

            await _db.ExecuteAsync(sql, new
            {
                AccountId = accountId,
                LastLoginAt = loginTime
            });
        }

        public async Task<Guid> UpdateStatusAsync(string username, AccountStatus status)
        {
            const string sql = @"
                UPDATE Accounts
                SET
                    Status = @Status,
                    UpdatedAt = SYSUTCDATETIME()
                OUTPUT INSERTED.AccountId
                WHERE Username = @Username;
            ";

            var accountId = await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    Username = username,
                    Status = status
                },
                Tx   // ⭐ 關鍵：一定要帶交易
            );

            return accountId;
        }

        public async Task UpdatePasswordAsync(string username, string passwordHash)
        {
            const string sql = @"
                UPDATE Accounts
                SET PasswordHash = @PasswordHash
                WHERE Username = @Username
            ";

            await _db.ExecuteAsync(sql, new
            {
                Username = username,
                PasswordHash = passwordHash
            });
        }

        public async Task UpdateUpdatedAtAsync(string username, DateTime updatedTime)
        {
            const string sql = @"
                UPDATE Accounts
                SET UpdatedAt = @UpdatedAt
                WHERE Username = @Username
            ";

            await _db.ExecuteAsync(sql, new
            {
                Username = username,
                UpdatedAt = updatedTime
            });
        }

        public async Task DeleteAccountAsync(string username)
        {
            const string sql = @"
                DELETE FROM Accounts
                WHERE Username = @Username
            ";

            await _db.ExecuteAsync(sql, new
            {
                Username = username,
            });
        }

        public async Task DeleteAccountRolesAsync(Guid accountId)
        {
            const string sql = @"
                DELETE FROM AccountRoles
                WHERE AccountId = @AccountId
            ";

            await _db.ExecuteAsync(
                sql,
                new { AccountId = accountId },
                transaction: Tx
            );
        }

        public async Task AddAccountRoleAsync(Guid accountId, Guid roleId)
        {
            const string sql = @"
                INSERT INTO AccountRoles 
                (
                    AccountId, 
                    RoleId, 
                    CreatedAt)
                VALUES 
                (
                    @AccountId,
                    @RoleId,
                    SYSUTCDATETIME()
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new { AccountId = accountId, RoleId = roleId },
                transaction: Tx
            );
        }
    }
}
