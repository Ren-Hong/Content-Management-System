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


        public async Task<IEnumerable<PermissionAssignmentSummaryEntity>> GetPermissionAssignmentSummariesAsync()
        {
            const string sql = @"
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
                JOIN Accounts a ON a.AccountId = dpa.AccountId
                JOIN Permissions p ON p.PermissionId = dpa.PermissionId
                JOIN Departments d ON d.DepartmentId = dpa.DepartmentId
                ORDER BY a.Username, p.PermissionName
            ";

            return await _db.QueryAsync<PermissionAssignmentSummaryEntity>(sql);
        }
    }
}
