using Cms.Application.Services.Role.Dtos;
using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Application.Services.Account.Dtos
{
    public class GetAccountSummariesResponseDto
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<GetRoleOptionsResponseDto> Roles { get; set; }
    }
}
