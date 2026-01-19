using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Infrastructure.Repositories.Account.Entities
{
    public class AccountSummaryEntity
    {
        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
