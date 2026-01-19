namespace Cms.Application.Services.Account.Dtos
{
    public class CreateAccountRequestDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string RoleCode {  get; set; } = string.Empty;
    }
}
