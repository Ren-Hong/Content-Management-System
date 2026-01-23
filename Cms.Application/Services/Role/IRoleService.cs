using Cms.Application.Services.Account.Dtos;
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

        /// <summary>
        /// 角色摘要table
        /// </summary>
        /// <returns></returns>
        Task<List<GetRoleSummariesResponseDto>> GetRoleSummariesAsync();

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<CreateRoleResponseDto> CreateRoleAsync(CreateRoleRequestDto req);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<UpdateRoleResponseDto> UpdateRoleAsync(UpdateRoleRequestDto req);

        /// <summary>
        /// 真實刪除角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<DeleteRoleResponseDto> DeleteRoleAsync(DeleteRoleRequestDto req);
    }
}
