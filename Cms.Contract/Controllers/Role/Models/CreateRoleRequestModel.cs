using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Role.Models
{
    public class CreateRoleRequestModel
    {
        [Required(ErrorMessage = "請輸入角色名稱")]
        public required string RoleName { get; set; }

        [Required(ErrorMessage = "請輸入角色代碼")]
        public required string RoleCode { get; set; }

        [Required(ErrorMessage = "請選擇權限")]
        [MinLength(1, ErrorMessage = "請至少選擇一個權限")]
        public required List<PermissionScopeModel> PermissionScopes { get; set; }
    }

    public class PermissionScopeModel
    {
        public required Guid PermissionId { get; set; }

        public required Guid ScopeId { get; set; }
    }
}
