namespace Cms.Contract.Repositories.Account.Entities
{
    public class AccountAuthEntity
    {
        public Guid AccountId { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public string? RoleCode { get; set; }

        public string? PermissionCode { get; set; }
    }
}
