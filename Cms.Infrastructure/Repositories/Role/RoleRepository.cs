using Cms.Contract.Repositories.Role.Entities;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Role.Persistence;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
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
                ORDER BY RoleName
            ";

            return await _db.QueryAsync<RoleOptionEntity>(
                sql,
                transaction: Tx
            );
        }

        public async Task<IEnumerable<RoleSummaryEntity>> GetRoleSummariesAsync()
        {
            const string sql = @"
                SELECT
                    r.RoleId,
                    r.RoleName,
                    p.PermissionId,
                    p.PermissionName,
                    r.Status
                FROM Roles r
                LEFT JOIN RolePermissions rp 
                    ON r.RoleId = rp.RoleId
                LEFT JOIN Permissions p 
                    ON rp.PermissionId = p.PermissionId
                ORDER BY r.RoleName;
            ";

            return await _db.QueryAsync<RoleSummaryEntity>(sql);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            const string sql = @"
                SELECT 1
                FROM Roles
                WHERE RoleName = @RoleName
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { RoleName = roleName }
            );

            return result != null; // 1 -> true, null -> false
        }

        public async Task<Guid> CreateRoleAsync(string roleName, string roleCode)
        {
            const string sql = @"
                INSERT INTO Roles
                (
                    RoleId,
                    RoleName,
                    RoleCode,
                    Status,
                    CreatedAt
                )
                OUTPUT INSERTED.RoleId
                VALUES
                (
                    NEWID(),
                    @RoleName,
                    @RoleCode,
                    1,
                    GETUTCDATE()
                );
            ";

            var roleId = await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    RoleName = roleName,
                    RoleCode = roleCode
                },
                transaction: Tx 
            );

            return roleId;
        }

        public async Task CreateRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            const string sql = @"
                INSERT INTO RolePermissions
                (
                    RoleId,
                    PermissionId
                )
                VALUES
                (
                    @RoleId,
                    @PermissionId
                );
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                },
                transaction: Tx
            );
        }

        public async Task<Guid> UpdateStatusAsync(string roleName, RoleStatus status)
        {
            const string sql = @"
                UPDATE Roles
                SET
                    Status = @Status,
                    UpdatedAt = SYSUTCDATETIME()
                OUTPUT INSERTED.RoleId
                WHERE RoleName = @RoleName;
            ";

            var roleId = await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    RoleName = roleName,
                    Status = status
                },
                Tx 
            );

            return roleId;
        }

        public async Task UpdateUpdatedAtAsync(string roleName, DateTime updatedTime)
        {
            const string sql = @"
                UPDATE Roles
                SET UpdatedAt = @UpdatedAt
                WHERE RoleName = @RoleName
            ";

            await _db.ExecuteAsync(sql, new
            {
                RoleName = roleName,
                UpdatedAt = updatedTime
            });
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            const string sql = @"
                DELETE FROM Roles
                WHERE RoleName = @RoleName
            ";

            await _db.ExecuteAsync(sql, new
            {
                RoleName = roleName,
            });
        }

        public async Task DeleteRolePermissionsAsync(Guid roleId)
        {
            const string sql = @"
                DELETE FROM RolePermissions
                WHERE RoleId = @RoleId
            ";

            await _db.ExecuteAsync(
                sql,
                new { RoleId = roleId },
                transaction: Tx
            );
        }

        public async Task AddRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            const string sql = @"
                INSERT INTO RolePermissions 
                (
                    RoleId,
                    PermissionId, 
                    CreatedAt
                )
                VALUES 
                (
                    @RoleId,
                    @PermissionId, 
                    SYSUTCDATETIME()
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new { RoleId = roleId, PermissionId = permissionId },
                transaction: Tx
            );
        }
    }
}
