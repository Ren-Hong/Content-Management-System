namespace Cms.Contract.Repositories.PermissionAssignment.Entities
{
    public class PermissionAssignmentSummaryEntity
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; } = default!;

        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = default!;

        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = default!;

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

    }
}
