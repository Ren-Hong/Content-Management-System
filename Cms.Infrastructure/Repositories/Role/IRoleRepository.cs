using Cms.Infrastructure.Repositories.Role.Entities;

namespace Cms.Infrastructure.Repositories.Role
{
    public interface IRoleRepository
    {
        /// <summary>
        /// 判斷該角色是否存在
        /// </summary>
        /// <param name="roleCode">前端傳過來的角色代碼(不是顯示用名子喔)</param>
        /// <returns>布林值</returns>
        Task<bool> RoleExistsAsync(string roleCode);

        /// <summary>
        /// 透過RoleCodes找RoleIds (前端傳多種角色)
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetRoleIdsByRoleCodesAsync(List<string> roleCodes);

        /// <summary>
        /// 給下拉用的role
        /// </summary>
        /// <returns>拿rolename跟rolecode</returns>
        Task<IEnumerable<RoleOptionEntity>> GetRoleOptionsAsync();
    }
}
