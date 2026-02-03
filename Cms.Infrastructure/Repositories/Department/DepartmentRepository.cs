using Cms.Contract.Repositories.Department.Entities;
using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Department
{
    public class DepartmentRepository : BaseRepository, IDepartmentRepository
    {
        public DepartmentRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> AllDepartmentsExistAsync(List<Guid> departmentIds)
        {
            const string sql = @"
                SELECT COUNT(DepartmentId)
                FROM Departments
                WHERE DepartmentId IN @DepartmentIds
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                new { DepartmentIds = departmentIds },
                transaction: Tx
            );

            return count == departmentIds.Count;
        }

        public async Task<IEnumerable<DepartmentOptionEntity>> GetDepartmentOptionsAsync()
        {
            const string sql = @"
                SELECT
                    DepartmentId,
                    DepartmentName
                FROM Departments
                WHERE Status = 1
                ORDER BY DepartmentName
            ";

            return await _db.QueryAsync<DepartmentOptionEntity>(
                sql,
                transaction: Tx
            );
        }
    }
}
