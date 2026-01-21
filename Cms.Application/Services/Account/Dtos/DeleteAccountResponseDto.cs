namespace Cms.Application.Services.Account.Dtos
{
    public class DeleteAccountResponseDto
    {
        public DeleteAccountResult Result { get; set; }
    }

    public enum DeleteAccountResult
    {
        Success,
        UsernameRequired,
        AccountNotFound
    }
}
