using Cms.Contract.Common.Pagination;
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

        public async Task<bool> AllRolesExistAsync(List<Guid> roleIds)
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

        public async Task<PagedResult<RoleSummaryEntity>> GetRoleSummariesPagedAsync(PageRequest preq)
        {
            var page = preq.Page <= 0 ? 1 : preq.Page;
            var pageSize = preq.PageSize <= 0 ? 10 : preq.PageSize;
            var offset = (page - 1) * pageSize;

            const string countSql = @"
                SELECT COUNT(*)
                FROM Roles;
            ";

            const string dataSql = @"
                -- 建立一張暫時存在的虛擬表:PagedRoles
                -- 這張表「只負責決定這一頁有哪些 Role」
                WITH PagedRoles AS (
                    SELECT RoleId
                    FROM Roles
                    ORDER BY RoleName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY -- offset前一定要orderby
                )
                SELECT
                    r.RoleId,
                    r.RoleName,

                    p.PermissionId,
                    p.PermissionName,

                    s.ScopeId,
                    s.ScopeCode,
                    s.ScopeName,

                    r.Status
                FROM Roles r
                -- 只留下剛剛分頁選中的 Role
                -- 等於說：只處理這一頁的 Role
                JOIN PagedRoles pr ON r.RoleId = pr.RoleId
                LEFT JOIN RolePermissionScopes rps ON r.RoleId = rps.RoleId
                LEFT JOIN Permissions p ON rps.PermissionId = p.PermissionId
                LEFT JOIN Scopes s ON rps.ScopeId = s.ScopeId
                ORDER BY r.RoleName, p.PermissionName;
            ";

            var totalCount = await _db.ExecuteScalarAsync<int>(countSql);

            var items = (await _db.QueryAsync<RoleSummaryEntity>(
                dataSql,
                new
                {
                    Offset = offset,
                    PageSize = pageSize
                }
            )).ToList();

            return new PagedResult<RoleSummaryEntity>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
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

        public async Task CreateRolePermissionScopeAsync(Guid roleId, Guid permissionId, Guid scopeId)
        {
            const string sql = @"
                INSERT INTO RolePermissionScopes
                (
                    RoleId,
                    PermissionId,
                    ScopeId
                )
                VALUES
                (
                    @RoleId,
                    @PermissionId,
                    @ScopeId
                );
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    ScopeId = scopeId
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
        
    }
}
