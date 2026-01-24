using Microsoft.AspNetCore.Authorization;

namespace Cms.Web.Authorization
{
    /// <summary>
    /// 權限授權處理器（Handler）
    ///
    /// 當授權系統遇到 PermissionRequirement 時
    /// 就會呼叫這個 Handler 來「實際檢查」
    ///
    /// 你整個系統的 Permission 判斷邏輯
    /// 都集中在這一個地方
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// HandleRequirementAsync = 授權系統真正執行檢查的地方
        /// </summary>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // context.User = 當前登入使用者（ClaimsPrincipal）
            // requirement.Permission = Controller 要求的權限字串

            // 檢查使用者的 Claims 裡
            // 是否有 permission = 指定的權限
            if (context.User.HasClaim("permission", requirement.Permission))
            {
                // 告訴授權系統：
                // 👉 這個 Requirement 已經被滿足
                context.Succeed(requirement);
            }

            // 不呼叫 Succeed = 不通過
            // ASP.NET Core 會自動回傳 403
            return Task.CompletedTask;
        }
    }
}
