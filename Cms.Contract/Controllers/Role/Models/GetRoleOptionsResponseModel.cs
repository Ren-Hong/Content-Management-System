namespace Cms.Contract.Controllers.Role.Models
{
    public class GetRoleOptionsResponseModel
    {
        public required Guid RoleId { get; set; }

        public required string RoleName { get; set; }
    }
}
