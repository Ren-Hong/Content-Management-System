using System.ComponentModel.DataAnnotations;

namespace Cms.Web.Controllers.Account.Models
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "請輸入使用者名稱")]
        [StringLength(20, ErrorMessage = "使用者名稱長度不可超過 20 字")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度需介於 8~20 字")]
        public required string Password { get; set; }
    }
}
