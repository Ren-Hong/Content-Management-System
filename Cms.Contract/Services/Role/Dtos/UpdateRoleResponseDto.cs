namespace Cms.Contract.Services.Role.Dtos
{
    public class UpdateRoleResponseDto
    {
        public UpdateRoleResult Result { get; set; }
    }

    public enum UpdateRoleResult
    {
        Success,
        RoleNameRequired,
        PermissionIdsRequired,
        RoleNameNotFound,
        StatusNotFound,
        PermissionNotFound
    }
}
