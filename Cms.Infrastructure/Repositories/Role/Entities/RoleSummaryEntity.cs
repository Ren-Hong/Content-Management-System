using Cms.Infrastructure.Repositories.Role.Persistence;

namespace Cms.Infrastructure.Repositories.Role.Entities
{
    public class RoleSummaryEntity
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public Guid? PermissionId { get; set; }

        public string? PermissionName { get; set; }

        public RoleStatus Status { get; set; }
    }
}
