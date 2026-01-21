using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Application.Services.Account.Dtos
{
    public class GetAccountSummariesResponseDto
    {
        public string Username { get; set; } = string.Empty;

        public AccountStatus Status { get; set; }

        public List<RoleResponseDto> Roles { get; set; } = new();
    }

    public class RoleResponseDto // for GetAccountSummaryResponseDto
    {
        public required string RoleCode { get; set; }
        public required string RoleName { get; set; }
    }
}
