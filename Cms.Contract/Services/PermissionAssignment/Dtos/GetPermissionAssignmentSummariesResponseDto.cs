namespace Cms.Contract.Services.PermissionAssignment.Dtos
{
    public class GetPermissionAssignmentSummariesResponseDto
    {
        public required Guid AccountId { get; set; }
        
        public required string Username { get; set; }

        public List<PermissionDepartmentsSummaryDto>? PermissionDepartments { get; set; }

    }

    public class PermissionDepartmentsSummaryDto
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public List<DepartmentSummaryDto>? Departments { get; set; }
    }

    public class DepartmentSummaryDto
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
