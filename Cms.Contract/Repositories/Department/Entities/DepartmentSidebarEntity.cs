namespace Cms.Contract.Repositories.Department.Entities
{
    public class DepartmentSidebarEntity
    {
        public Guid DepartmentId { get; set; }
        
        public required string DepartmentCode { get; set; }

        public required string DepartmentName { get; set; }
    }
}
