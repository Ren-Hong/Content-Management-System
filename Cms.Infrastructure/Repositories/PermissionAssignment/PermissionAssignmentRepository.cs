using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.PermissionAssignment.Entities;
using Cms.Contract.Repositories.PermissionAssignment.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.PermissionAssignment
{
    public class PermissionAssignmentRepository : BaseRepository, IPermissionAssignmentRepository
    {
        public PermissionAssignmentRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<PagedResult<PermissionAssignmentSummaryEntity>> GetPermissionAssignmentSummariesPagedAsync(PageRequest preq)
        {
            var page = preq.Page <= 0 ? 1 : preq.Page;
            var pageSize = preq.PageSize <= 0 ? 10 : preq.PageSize;
            var offset = (page - 1) * pageSize;

            // ================== 計算 Account 總數 ==================
            const string countSql = @"
                SELECT COUNT(DISTINCT AccountId)
                FROM DepartmentPermissionAssignments
            ";

            // ================== 分頁 Account ==================
            const string dataSql = @"
                WITH PagedAccounts AS (
                    SELECT DISTINCT 
                        dpa.AccountId,
                        a.Username
                    FROM DepartmentPermissionAssignments dpa
                    JOIN Accounts a ON a.AccountId = dpa.AccountId
                    ORDER BY a.Username
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT
                    dpa.AccountId,
                    a.Username,

                    dpa.PermissionId,
                    p.PermissionName,

                    dpa.DepartmentId,
                    d.DepartmentName,

                    dpa.ValidFrom,
                    dpa.ValidTo
                FROM DepartmentPermissionAssignments dpa
                JOIN PagedAccounts pa ON pa.AccountId = dpa.AccountId
                JOIN Accounts a ON a.AccountId = dpa.AccountId
                JOIN Permissions p ON p.PermissionId = dpa.PermissionId
                JOIN Departments d ON d.DepartmentId = dpa.DepartmentId
                ORDER BY a.Username, p.PermissionName
            ";

            var totalCount = await _db.ExecuteScalarAsync<int>(countSql);

            var items = (await _db.QueryAsync<PermissionAssignmentSummaryEntity>(
                dataSql,
                new
                {
                    Offset = offset,
                    PageSize = pageSize
                }
            )).ToList();

            return new PagedResult<PermissionAssignmentSummaryEntity>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
