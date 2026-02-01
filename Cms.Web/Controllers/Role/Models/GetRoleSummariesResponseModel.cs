using Cms.Contract.Repositories.Role.Persistence;
using Cms.Web.Controllers.Permission.Models;

namespace Cms.Web.Controllers.Role.Models
{
    public class GetRoleSummariesResponseModel
    {
        
            public required Guid RoleId { get; set; }

            public required string RoleName { get; set; }

            public RoleStatus Status { get; set; }

            public List<PermissionScopesSummaryModel>? PermissionScopes { get; set; }
    }

    public class PermissionScopesSummaryModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public required List<ScopeSummaryModel> Scopes { get; set; }
    }

    public class ScopeSummaryModel
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }
    }
}
