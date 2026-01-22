using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Infrastructure.Repositories.Account.Entities
{
    public class AccountSummaryEntity
    {
        public required string Username { get; set; } 

        public Guid? RoleId { get; set; } 

        public string? RoleName { get; set; } 

        public AccountStatus Status { get; set; }
    }
}
