using Cms.Contract.Services.Role.Dtos;
using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Services.Account.Dtos
{
    public class GetAccountSummariesResponseDto
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<GetRoleOptionsResponseDto> Roles { get; set; }
    }
}
