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
        /// 透過RoleCode找RoleId
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        Task<Guid?> GetRoleIdByCodeAsync(string roleCode);

        /// <summary>
        /// 給下拉用的role
        /// </summary>
        /// <returns>拿rolename跟rolecode</returns>
        Task<IEnumerable<RoleOptionEntity>> GetRoleOptionsAsync();
    }
}
