using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Services.Account.Dtos
{
    public class UpdateAccountRequestDto
    {
        public required string Username { get; set; }

        public required List<Guid> RoleIds { get; set; }

        public AccountStatus Status { get; set; }
    }
}
