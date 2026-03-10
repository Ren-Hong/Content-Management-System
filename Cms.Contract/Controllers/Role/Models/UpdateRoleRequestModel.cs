using Cms.Contract.Repositories.Role.Persistence;
using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Role.Models
{
    public class UpdateRoleRequestModel
    {
        [Required(ErrorMessage = "請輸入角色名稱")]
        public required string RoleName { get; set; }

        [Required(ErrorMessage = "請選擇權限")]
        [MinLength(1, ErrorMessage = "請至少選擇一個權限")]
        public required List<PermissionScopeModel> PermissionScopes { get; set; }

        [Required(ErrorMessage = "請選擇狀態")]
        public RoleStatus Status { get; set; }
    }
}
