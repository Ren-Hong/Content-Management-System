namespace Cms.Contract.Services.Account.Dtos
{
    public class UpdateAccountResponseDto
    {
        public UpdateAccountResult Result { get; set; }
    }

    public enum UpdateAccountResult
    {
        Success,
        UsernameRequired,
        RoleIdsRequired,
        AccountNotFound,
        StatusNotFound,
        RoleNotFound
    }
}
