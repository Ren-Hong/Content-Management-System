using Cms.Application.Services.Role.Dtos;

namespace Cms.Application.Services.Role
{
    public interface IRoleService
    {
        /// <summary>
        /// 專給下拉用的Role
        /// </summary>
        /// <returns></returns>
        Task<List<GetRoleOptionsResponseDto>> GetRoleOptionsAsync();
    }
}
