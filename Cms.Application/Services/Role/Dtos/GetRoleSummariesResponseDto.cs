using Cms.Infrastructure.Repositories.Role.Persistence;
using Cms.Application.Services.Permission.Dtos;

namespace Cms.Application.Services.Role.Dtos
{
    public class GetRoleSummariesResponseDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public required List<GetPermissionOptionsResponseDto> Permissions { get; set; }
    }
}
