using Cms.Contract.Services.Department.Dtos;

namespace Cms.Contract.Services.Department.Interfaces
{
    public interface IDepartmentService
    {
        /// <summary>
        /// 專給下拉用的Department
        /// </summary>
        /// <returns></returns>
        Task<List<GetDepartmentOptionsResponseDto>> GetDepartmentOptionsAsync();

        /// <summary>
        /// 專給側邊欄用的Department
        /// </summary>
        /// <returns></returns>
        Task<List<GetDepartmentsForSidebarResponseDto>> GetDepartmentsForSidebarAsync(Guid accountId);
    }
}
