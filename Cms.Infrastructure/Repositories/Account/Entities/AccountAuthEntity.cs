using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Infrastructure.Repositories.Account.Entities
{
    public class AccountAuthEntity
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? RoleCode { get; set; }
        public string? PermissionCode { get; set; }
    }
}
