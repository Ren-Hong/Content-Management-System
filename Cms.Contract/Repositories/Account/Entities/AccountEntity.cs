using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Repositories.Account.Entities
{
    public class AccountEntity
    {
        public Guid AccountId { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public required string Role { get; set; } //理論上創建帳戶的時候一定要配一個角色

        public AccountStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
