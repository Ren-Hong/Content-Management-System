using Cms.Application.Services.Domain;

namespace Cms.Application.Services.Account.Dtos
{
    public class UpdateAccountRequestDto
    {
        public required string Username { get; set; }

        public required string RoleCode { get; set; }

        public AccountStatus Status { get; set; }
    }
}
