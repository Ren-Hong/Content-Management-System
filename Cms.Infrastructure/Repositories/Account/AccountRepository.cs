using Cms.Infrastructure.Repositories.Account.Entities;
using Cms.Infrastructure.Repositories.Base;
using Cms.Infrastructure.Repositories.UnitOfWork;
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
                LEFT JOIN AccountRoles ar ON a.AccountId = ar.AccountId
                LEFT JOIN Roles r ON ar.RoleId = r.RoleId
                LEFT JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                LEFT JOIN Permissions p ON rp.PermissionId = p.PermissionId
                WHERE a.Username = @Username
                  AND a.Status = 'Active';
            ";

            return await _db.QueryAsync<AccountAuthEntity>(
                sql,
                new { Username = username }
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

        public async Task<IEnumerable<AccountSummaryEntity>> GetAccountSummariesAsync()
        {
            const string sql = @"
                SELECT
                    a.Username,
                    r.RoleCode AS Role,
                    a.Status
                FROM Accounts a
                LEFT JOIN AccountRoles ar ON a.AccountId = ar.AccountId
                LEFT JOIN Roles r ON ar.RoleId = r.RoleId
                ORDER BY a.Username;
            ";

            return await _db.QueryAsync<AccountSummaryEntity>(sql);
        }

        public async Task<bool> UsernameExistsAsync(string username)
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
                    'Active',
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

        public async Task AssignRoleAsync(Guid accountId, Guid roleId)
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
    }
}
