namespace Cms.Contract.Services.Department.Dtos
{
    public class GetDepartmentsForSidebarResponseDto
    {
        public Guid DepartmentId { get; set; }

        public required string DepartmentCode { get; set; }

        public required string DepartmentName { get; set; }
    }
}