using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Cms.Web.Authorization
{
    /// <summary>
    /// 自訂 Policy Provider
    ///
    /// 目的：
    /// 👉 不用在 Program.cs 寫幾百個 AddPolicy
    ///
    /// 當 Controller 使用：
    /// [Authorize(Policy = "Permission.IT.Edit")]
    ///
    /// ASP.NET Core 會來問這個 Provider：
    /// 「這個 Policy 是什麼？」
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        // 我們自訂的 Policy 命名規則前綴
        // 只要是 Permission.xxx
        // 就由這個 Provider 處理
        private const string POLICY_PREFIX = "Permission.";

        // 內建的預設 Provider
        // 用來處理不是 Permission 開頭的 Policy
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // 保留內建行為（例如 Roles、預設 Policy）
            _fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        /// <summary>
        /// 當授權系統看到一個 Policy 名稱時
        /// 會呼叫這個方法來「取得 Policy 定義」
        /// </summary>
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // 如果 Policy 名稱是 Permission.xxx
            if (policyName.StartsWith(POLICY_PREFIX))
            {
                // 把 xxx 拿出來
                // 例如 Permission.IT.Edit → IT.Edit
                var permission = policyName.Substring(POLICY_PREFIX.Length);

                // 動態建立一個 Policy
                // 裡面只放一個 PermissionRequirement
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // 不是我們管的 Policy（例如 Roles）
            // 交回給內建 Provider 處理
            return _fallback.GetPolicyAsync(policyName);
        }

        // 下面兩個方法只是轉交給內建 Provider
        // 通常不需要改

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallback.GetFallbackPolicyAsync();
    }
}

