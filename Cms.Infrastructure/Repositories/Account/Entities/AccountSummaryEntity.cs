using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Infrastructure.Repositories.Account.Entities
{
    public class AccountSummaryEntity
    {
        public required string Username { get; set; } 

        public required string RoleCode { get; set; } 

        public required string RoleName { get; set; } 

        public AccountStatus Status { get; set; }
    }
}
