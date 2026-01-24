using Cms.Contract.Repositories.Permission.Entities;

namespace Cms.Contract.Repositories.Permission.Interfaces
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// 判斷該權限是否存在
        /// </summary>
        /// <param name="permissionIds">前端傳過來的權限Id</param>
        /// <returns>bool</returns>
        Task<bool> PermissionIdsExistAsync(List<Guid> permissionIds);

        /// <summary>
        /// 給下拉用的permission
        /// </summary>
        /// <returns>拿rolename跟rolecode</returns>
        Task<IEnumerable<PermissionOptionEntity>> GetPermissionOptionsAsync();


    }
}
