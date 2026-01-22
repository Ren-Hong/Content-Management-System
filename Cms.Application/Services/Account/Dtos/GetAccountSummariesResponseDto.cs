using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Application.Services.Account.Dtos
{
    public class GetAccountSummariesResponseDto
    {
        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<RoleResponseDto> Roles { get; set; }
    }

    public class RoleResponseDto // for GetAccountSummaryResponseDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}
