using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Services.Role.Dtos
{
    public class UpdateRoleRequestDto
    {
        public required string RoleName { get; set; }

        public required List<Guid> PermissionIds { get; set; }

        public RoleStatus Status { get; set; }
    }
}
