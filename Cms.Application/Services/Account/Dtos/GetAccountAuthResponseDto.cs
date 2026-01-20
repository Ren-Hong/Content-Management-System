using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Application.Services.Account.Dtos
{
    public class GetAccountAuthResponseDto
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] Permissions { get; set; } = Array.Empty<string>();
    }
}
