using System.ComponentModel.DataAnnotations;

namespace Cms.Web.Models.Account
{
    public class CreateAccountRequestModel
    {
        [Required(ErrorMessage = "請輸入使用者名稱")]
        [StringLength(20, ErrorMessage = "使用者名稱長度不可超過 20 字")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度需介於 8~20 字")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇角色")]
        public string RoleCode { get; set; }
    }
}
