using Cms.Infrastructure.Repositories.Role.Persistence;

namespace Cms.Application.Services.Role.Dtos
{
    public class GetRoleSummariesResponseDto
    {
        public required string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public List<PermissionResponseDto> Permissions { get; set; } = new();
    }

    public class PermissionResponseDto 
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
