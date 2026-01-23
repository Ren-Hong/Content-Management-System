namespace Cms.Infrastructure.Repositories.Permission.Entities
{
    public class PermissionOptionEntity
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
