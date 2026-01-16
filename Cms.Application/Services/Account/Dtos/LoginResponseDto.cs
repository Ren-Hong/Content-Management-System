namespace Cms.Application.Services.Account.Dtos
{
    public class LoginResponseDto
    {
        public LoginResult Result { get; set; }

        public Guid? AccountId { get; set; }

        public string? Username { get; set; }

        public string[]? Roles { get; set; }

        public string[]? Permissions { get; set; }
    }

    public enum LoginResult
    {
        Success,
        AccountNotFound,
        InvalidPassword
    }
}
