namespace Cms.Contract.Controllers.Permission.Models
{
    public class GetPermissionOptionsResponseModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
