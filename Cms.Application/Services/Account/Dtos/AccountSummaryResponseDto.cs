using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Application.Services.Account.Dtos
{
    public class AccountSummaryResponseDto
    {
        public string Username { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
    }
}
