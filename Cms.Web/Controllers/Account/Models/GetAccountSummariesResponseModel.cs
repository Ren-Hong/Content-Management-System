using Cms.Infrastructure.Repositories.Account.Persistence;

namespace Cms.Web.Controllers.Account.Models
{
    public class GetAccountSummariesResponseModel
    {
        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public List<RoleResponseModel> Roles { get; set; } = new();
    }

    public class RoleResponseModel
    {
        public required string RoleCode { get; set; }
        public required string RoleName { get; set; }
    }
}
