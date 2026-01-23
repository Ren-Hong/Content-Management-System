using Cms.Application.Services.Permission.Dtos;

namespace Cms.Application.Services.Permission
{
    public interface IPermissionService
    {
        /// <summary>
        /// 專給下拉用的Permission
        /// </summary>
        /// <returns></returns>
        Task<List<GetPermissionOptionsResponseDto>> GetPermissionOptionsAsync();
    }
}
