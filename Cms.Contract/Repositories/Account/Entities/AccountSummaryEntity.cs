using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Repositories.Account.Entities
{
    public class AccountSummaryEntity
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; } 

        public Guid? RoleId { get; set; } 

        public string? RoleName { get; set; } 

        public AccountStatus Status { get; set; }
    }
}
