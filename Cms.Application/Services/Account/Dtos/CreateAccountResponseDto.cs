namespace Cms.Application.Services.Account.Dtos
{
    public class CreateAccountResponseDto
    {
        public CreateAccountResult Result { get; set; }
    }

    public enum CreateAccountResult
    {
        Success,
        InvalidRequest,
        UsernameRequired,
        PasswordRequired,
        UsernameAlreadyExists,
        RoleRequired,
        RoleNotExist
    }
}
