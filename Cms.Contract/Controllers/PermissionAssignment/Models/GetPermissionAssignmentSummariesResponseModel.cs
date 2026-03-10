namespace Cms.Contract.Controllers.PermissionAssignment.Models
{
    public class GetPermissionAssignmentSummariesResponseModel
    {
        public required Guid AccountId { get; set; }
        
        public required string Username { get; set; }

        public List<PermissionDepartmentsSummaryResponseModel>? PermissionDepartments { get; set; }

    }

    public class PermissionDepartmentsSummaryResponseModel
    {
        public required Guid PermissionId { get; set; }

        public required string PermissionName { get; set; }

        public List<DepartmentSummaryResponseModel>? Departments { get; set; }
    }

    public class DepartmentSummaryResponseModel
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
