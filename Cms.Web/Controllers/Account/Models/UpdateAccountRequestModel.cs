using Cms.Contract.Repositories.Account.Persistence;
using System.ComponentModel.DataAnnotations;

namespace Cms.Web.Controllers.Account.Models
{
    public class UpdateAccountRequestModel
    {
        [Required(ErrorMessage = "請輸入使用者名稱")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "請選擇角色")]
        public required List<Guid> RoleIds { get; set; }

        [Required(ErrorMessage = "請選擇狀態")]
        public AccountStatus Status { get; set; }
    }
}
