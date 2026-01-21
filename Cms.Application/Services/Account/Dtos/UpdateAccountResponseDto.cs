namespace Cms.Application.Services.Account.Dtos
{
    public class UpdateAccountResponseDto
    {
        public UpdateAccountResult Result { get; set; }
    }

    public enum UpdateAccountResult
    {
        Success,
        UsernameRequired,
        RoleCodesRequired,
        AccountNotFound,
        StatusNotFound,
        RoleNotFound
    }
}
