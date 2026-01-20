using Cms.Application.Services.Domain;

namespace Cms.Web.Controllers.Account.Models
{
    public class GetAccountSummariesResponseModel
    {
        public string Username { get; set; }

        public string Status { get; set; }

        public List<RoleResponseModel> Roles { get; set; } = new();
    }

    public class RoleResponseModel // for GetAccountSummariesResponseModel
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
    }
}
