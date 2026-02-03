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
    }
}
