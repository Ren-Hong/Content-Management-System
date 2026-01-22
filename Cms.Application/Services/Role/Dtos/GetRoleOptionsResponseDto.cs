namespace Cms.Application.Services.Role.Dtos
{
    public class GetRoleOptionsResponseDto
    {
        public required Guid RoleId { get; set; } 

        public required string RoleName { get; set; }
    }
}
