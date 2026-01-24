using Cms.Contract.Services.Permission.Dtos;

namespace Cms.Contract.Services.Permission.Interfaces
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
