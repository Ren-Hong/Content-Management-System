namespace Cms.Application.Services.Role.Dtos
{
    public class DeleteRoleResponseDto
    {
        public DeleteRoleResult Result { get; set; }
    }

    public enum DeleteRoleResult
    {
        Success,
        RoleNameRequired,
        RoleNotFound
    }
}
