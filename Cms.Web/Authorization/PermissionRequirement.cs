using Microsoft.AspNetCore.Authorization;

namespace Cms.Web.Authorization
{
        /// <summary>
        /// 一個「授權需求（Requirement）」
        ///
        /// 這個類別本身【不做判斷】
        /// 它只負責描述：
        /// 👉「需要某一個 permission 字串」
        ///
        /// ASP.NET Core 授權系統的設計是：
        /// - Requirement：描述需求（是什麼）
        /// - Handler：實作判斷（怎麼檢查）
        /// </summary>
        public class PermissionRequirement : IAuthorizationRequirement
        {
            /// <summary>
            /// Controller 要求的權限字串
            /// 例如：IT.Edit、HR.View
            /// </summary>
            public string Permission { get; }

            public PermissionRequirement(string permission)
            {
                // 這裡只是單純存起來
                // 真正的判斷會在 Handler 裡做
                Permission = permission;
            }
        }
    }
}