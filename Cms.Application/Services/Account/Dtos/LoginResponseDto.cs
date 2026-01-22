namespace Cms.Application.Services.Account.Dtos
{
    public class LoginResponseDto
    {
        public LoginResult Result { get; set; }

        public Guid? AccountId { get; set; }

        public string? Username { get; set; }

        public List<string>? RoleCodes { get; set; }

        public List<string>? PermissionCodes { get; set; }
    }

    public enum LoginResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        AccountNotFound,
        InvalidPassword
    }
}
