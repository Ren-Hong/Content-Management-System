namespace Cms.Contract.Repositories.Department.Entities
{
    public class DepartmentOptionEntity
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}
