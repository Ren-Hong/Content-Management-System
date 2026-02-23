using Cms.Contract.Repositories.Account.Persistence;
using Cms.Web.Controllers.Department.Models;
using Cms.Web.Controllers.Role.Models;

namespace Cms.Web.Controllers.Account.Models
{
    public class GetAccountSummariesResponseModel
    {
        public required Guid AccountId { get; set; }

        public required string Username { get; set; }

        public AccountStatus Status { get; set; }

        public required List<AccountRoleAssignmentResponseModel> RoleAssignments { get; set; }
    }

    public class AccountRoleAssignmentResponseModel
    {
        public required Guid RoleId { get; set; }
        
        public required string RoleName { get; set; }

        public required List<GetDepartmentOptionsResponseModel> Departments { get; set; }
    }
}
