namespace Cms.Contract.Services.Department.Dtos
{
    public class GetDepartmentOptionsResponseDto
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}
