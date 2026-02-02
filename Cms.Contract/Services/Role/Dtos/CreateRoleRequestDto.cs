namespace Cms.Contract.Services.Role.Dtos
{
    public class CreateRoleRequestDto
    {
        public required string RoleName { get; set; }

        public required string RoleCode { get; set; }

        public required List<PermissionScopeDto> PermissionScopes { get; set; }
    }

    public class PermissionScopeDto
    {
        public required Guid PermissionId { get; set; }

        public required Guid ScopeId { get; set; }
    }
}
