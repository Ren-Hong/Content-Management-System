using System.ComponentModel.DataAnnotations;

namespace Cms.Web.Controllers.Role.Models
{
    public class CreateRoleRequestModel
    {
        [Required(ErrorMessage = "請輸入角色名稱")]
        public required string RoleName { get; set; }

        [Required(ErrorMessage = "請輸入角色代碼")]
        public required string RoleCode { get; set; }

        [Required(ErrorMessage = "請選擇權限")]
        public required List<Guid> PermissionIds { get; set; }
    }
}
