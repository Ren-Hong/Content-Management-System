using Cms.Infrastructure.Repositories.Account.Persistence;
using Cms.Web.Controllers.Role.Models;

namespace Cms.Web.Controllers.Account.Models
{
    public class GetAccountSummariesResponseModel
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<GetRoleOptionsResponseModel> Roles { get; set; }
    }
}
