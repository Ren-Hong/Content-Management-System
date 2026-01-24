namespace Cms.Contract.Services.Account.Dtos
{
    public class ResetPasswordResponseDto
    {
        public ResetPasswordResult Result { get; set; }
    }

    public enum ResetPasswordResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        AccountNotFound
    }
}
