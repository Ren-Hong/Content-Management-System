using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Services.Role.Dtos
{
    public class GetRoleSummariesResponseDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public required List<PermissionScopesSummaryDto> PermissionScopes { get; set; }
    }

    public class PermissionScopesSummaryDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public required List<ScopeSummaryDto> Scopes { get; set; }
    }

    public class ScopeSummaryDto
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }
    }
}
