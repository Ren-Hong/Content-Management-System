using Cms.Infrastructure.Repositories.Account.Entities;

namespace Cms.Infrastructure.Repositories.Account
{
    /// <summary>
    /// 帳號資料存取介面（Accounts）
    /// <para>
    /// 僅負責與資料庫進行互動，不包含任何商業邏輯或流程判斷。
    /// </para>
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// 依帳號識別碼取得帳號資料
        /// </summary>
        /// <param name="accountId">帳號唯一識別碼</param>
        /// <returns>
        /// 帳號資料；若查無資料則回傳 <c>null</c>
        /// </returns>
        Task<IEnumerable<AccountAuthEntity>> GetAccountAuthByUsernameAsync(string username);

        /// <summary>
        /// 更新帳號最後登入時間
        /// </summary>
        /// <param name="accountId">帳號唯一識別碼</param>
        /// <param name="loginTime">登入時間（UTC）</param>
        Task UpdateLastLoginAtAsync(Guid accountId, DateTime loginTime);

        /// <summary>
        /// 取得所有帳戶摘要
        /// </summary>
        Task<IEnumerable<AccountSummaryEntity>> GetAccountSummariesAsync();

        /// <summary>
        /// 判斷帳號是否已存在
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// 建立帳戶
        /// </summary>
        /// <param name="Username">帳戶</param>
        /// <param name="passwordHash">密碼</param>
        /// <returns>回Guid, 因為後面要直接加角色</returns>
        Task<Guid> CreateAccountAsync(string Username, string passwordHash);

        /// <summary>
        /// 幫帳戶加角色(不是只有建帳戶的時候用)
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleCode"></param>
        /// <returns>不用回傳值, 失敗就是例外</returns>
        Task AssignRoleAsync(Guid accountId, Guid roleId);
    }
}
