using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Account.Models
{
    public class ResetPasswordRequestModel
    {
        public required string Username { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度需介於 8~20 字")]
        public required string Password { get; set; }
    }
}
