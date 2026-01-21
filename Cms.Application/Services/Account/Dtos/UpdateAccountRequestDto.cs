using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Application.Services.Account.Dtos
{
    public class UpdateAccountRequestDto
    {
        public required string Username { get; set; }

        public required List<string> RoleCodes { get; set; }

        public AccountStatus Status { get; set; }
    }
}
