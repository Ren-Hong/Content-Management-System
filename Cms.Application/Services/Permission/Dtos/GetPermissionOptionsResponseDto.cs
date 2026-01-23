namespace Cms.Application.Services.Permission.Dtos
{
    public class GetPermissionOptionsResponseDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }
    }
}
