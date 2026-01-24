namespace Cms.Contract.Services.Account.Dtos
{
    public class CreateAccountRequestDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public required List<Guid> RoleIds { get; set; }
    }
}
