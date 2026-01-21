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

        public async Task<IEnumerable<Guid>> GetRoleIdsByRoleCodesAsync(List<string> roleCodes)
        {
            // 這邊一次驗最棒, 不用foreach , 有Where IN語法
            // Dapper 會幫你轉成 WHERE RoleCode IN (@RoleCodes1, @RoleCodes2, @RoleCodes3)
            const string sql = @"
                SELECT RoleId
                FROM Roles
                WHERE RoleCode IN @RoleCodes
            ";

            var roleIds = await _db.QueryAsync<Guid>(
                sql,
                new { RoleCodes = roleCodes },
                transaction: Tx
            );

            return roleIds;
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
