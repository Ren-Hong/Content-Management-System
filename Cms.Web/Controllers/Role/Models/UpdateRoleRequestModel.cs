using Cms.Contract.Repositories.Role.Persistence;
using System.ComponentModel.DataAnnotations;

namespace Cms.Web.Controllers.Role.Models
{
    public class UpdateRoleRequestModel
    {
        [Required(ErrorMessage = "請輸入角色名稱")]
        public required string RoleName { get; set; }

        [Required(ErrorMessage = "請選擇權限")]
        public required List<Guid> PermissionIds { get; set; }

        [Required(ErrorMessage = "請選擇狀態")]
        public RoleStatus Status { get; set; }
    }
}
