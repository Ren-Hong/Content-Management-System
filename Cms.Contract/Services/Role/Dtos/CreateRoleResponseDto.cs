using Cms.Contract.Services.Account.Dtos;

namespace Cms.Contract.Services.Role.Dtos
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
        PermissionRequired,
        PermissionNotFound,
        ScopeNotFound
    }
}
