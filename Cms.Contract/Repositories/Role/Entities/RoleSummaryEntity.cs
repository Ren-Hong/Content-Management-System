using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Repositories.Role.Entities
{
    public class RoleSummaryEntity
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public Guid? PermissionId { get; set; }

        public string? PermissionName { get; set; }

        public Guid? ScopeId { get; set; }

        public string? ScopeName { get; set; }

        public RoleStatus Status { get; set; }
    }
}
