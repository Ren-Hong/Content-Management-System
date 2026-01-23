using Cms.Infrastructure.Repositories.Role.Persistence;

namespace Cms.Application.Services.Role.Dtos
{
    public class UpdateRoleRequestDto
    {
        public required string RoleName { get; set; }

        public required List<Guid> PermissionIds { get; set; }

        public RoleStatus Status { get; set; }
    }
}
