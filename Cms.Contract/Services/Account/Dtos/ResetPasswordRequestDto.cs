namespace Cms.Contract.Services.Account.Dtos
{
    public class ResetPasswordRequestDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}
