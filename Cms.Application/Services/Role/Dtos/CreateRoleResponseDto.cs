using Cms.Application.Services.Account.Dtos;

namespace Cms.Application.Services.Role.Dtos
{
    public class CreateRoleResponseDto
    {
        public CreateRoleResult Result { get; set; }
    }

    public enum CreateRoleResult
    {
        Success,
        RoleNameRequired,
        RoleCodeRequired,
        RoleNameDuplicated,
        PermissionIdsRequired,
        PermissionNotFound
    }

}
