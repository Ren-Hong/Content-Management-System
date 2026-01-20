using Cms.Infrastructure.Repositories.Account;
using Cms.Infrastructure.Repositories.Base;
using Cms.Infrastructure.Repositories.Role.Entities;
using Cms.Infrastructure.Repositories.UnitOfWork;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Role
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> RoleExistsAsync(string roleCode)
        {
            const string sql = @"
                SELECT 1
                FROM Roles
                WHERE RoleCode = @RoleCode
            ";

            return await _db.ExecuteScalarAsync<int?>(sql, new { RoleCode = roleCode }) != null; //ExecuteScalarAsync<T> 只回一個值
        }

        public async Task<Guid?> GetRoleIdByCodeAsync(string roleCode)
        {
            const string sql = @"
                SELECT RoleId
                FROM Roles
                WHERE RoleCode = @RoleCode
            ";

            return await _db.ExecuteScalarAsync<Guid?>(
                sql,
                new { RoleCode = roleCode },
                transaction: Tx   // ⚠️ 一樣接交易
            );
        }

        public async Task<IEnumerable<RoleOptionEntity>> GetRoleOptionsAsync()
        {
            const string sql = @"
                SELECT
                    RoleCode,
                    RoleName
                FROM Roles
                WHERE Status = 1
                ORDER BY CreatedAt
            ";

            return await _db.QueryAsync<RoleOptionEntity>(sql);
        }
    }
}
