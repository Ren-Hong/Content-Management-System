using Cms.Contract.Repositories.Permission.Entities;
using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Permission
{
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        public PermissionRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> AllPermissionsExistAsync(List<Guid> permissionIds)
        {
            const string sql = @"
                SELECT COUNT(PermissionId)
                FROM Permissions
                WHERE PermissionId IN @PermissionIds
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                new { PermissionIds = permissionIds },
                transaction: Tx
            );

            return count == permissionIds.Count;
        }

        public async Task<IEnumerable<PermissionOptionEntity>> GetPermissionOptionsAsync()
        {
            const string sql = @"
                SELECT
                    PermissionId,
                    PermissionName
                FROM Permissions
                WHERE Status = 1
                ORDER BY PermissionName
            ";

            return await _db.QueryAsync<PermissionOptionEntity>(
                sql,
                transaction: Tx
            );
        }
    }
}
