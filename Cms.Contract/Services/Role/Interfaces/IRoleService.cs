using Cms.Contract.Services.Role.Dtos;

namespace Cms.Contract.Services.Role.Interfaces
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
