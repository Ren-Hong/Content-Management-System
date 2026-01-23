using Cms.Infrastructure.Repositories.Permission.Entities;
using Cms.Infrastructure.Repositories.Role.Entities;

namespace Cms.Infrastructure.Repositories.Permission
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
