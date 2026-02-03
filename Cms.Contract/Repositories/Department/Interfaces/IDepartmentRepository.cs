using Cms.Contract.Repositories.Department.Entities;

namespace Cms.Contract.Repositories.Department.Interfaces
{
    public interface IDepartmentRepository
    {
        /// <summary>
        /// 判斷該部門是否存在
        /// </summary>
        /// <param name="departmentIds">前端傳過來的權限Id</param>
        /// <returns>bool</returns>
        Task<bool> AllDepartmentsExistAsync(List<Guid> departmentIds);

        /// <summary>
        /// 給下拉用的Department
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DepartmentOptionEntity>> GetDepartmentOptionsAsync();
    }
}
