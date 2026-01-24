using Cms.Contract.Repositories.Role.Persistence;
using Cms.Contract.Services.Permission.Dtos;

namespace Cms.Contract.Services.Role.Dtos
{
    public class GetRoleSummariesResponseDto
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public required List<GetPermissionOptionsResponseDto> Permissions { get; set; }
    }
}
