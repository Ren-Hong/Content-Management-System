namespace Cms.Application.Services.Role.Dtos
{
    public class CreateRoleRequestDto
    {
        public required string RoleName { get; set; }

        public required string RoleCode { get; set; }

        public required List<Guid> PermissionIds { get; set; }
    }
}
