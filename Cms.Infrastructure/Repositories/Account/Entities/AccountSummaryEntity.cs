using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Infrastructure.Repositories.Account.Entities
{
    public class AccountSummaryEntity
    {
        public required string Username { get; set; } 

        public required string RoleCode { get; set; } 

        public required string RoleName { get; set; } 

        public short Status { get; set; }
    }
}
