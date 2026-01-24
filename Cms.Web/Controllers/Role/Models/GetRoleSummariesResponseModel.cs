using Cms.Contract.Repositories.Role.Persistence;
using Cms.Web.Controllers.Permission.Models;

namespace Cms.Web.Controllers.Role.Models
{
    public class GetRoleSummariesResponseModel
    {
        
            public required Guid RoleId { get; set; }

            public required string RoleName { get; set; }

            public RoleStatus Status { get; set; }

            public List<GetPermissionOptionsResponseModel>? Permissions { get; set; }
    }
}
