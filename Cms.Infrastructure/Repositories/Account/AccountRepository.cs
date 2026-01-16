using Cms.Infrastructure.Repositories.Account.Entities;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _db;

        public AccountRepository(IDbConnection db)
        {
            _db = db;
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
    }
}
