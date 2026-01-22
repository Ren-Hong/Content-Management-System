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

        public async Task<bool> RoleIdsExistAsync(List<Guid> roleIds)
        {
            const string sql = @"
                SELECT COUNT(RoleId)
                FROM Roles
                WHERE RoleId IN @RoleIds
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                new { RoleIds = roleIds },
                transaction: Tx
            );

            return count == roleIds.Count;
        }

        public async Task<IEnumerable<RoleOptionEntity>> GetRoleOptionsAsync()
        {
            const string sql = @"
                SELECT
                    RoleId,
                    RoleName
                FROM Roles
                WHERE Status = 1
                ORDER BY CreatedAt
            ";

            return await _db.QueryAsync<RoleOptionEntity>(
                sql,
                transaction: Tx
            );
        }
    }
}
