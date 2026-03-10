namespace Cms.Contract.Controllers.Department.Models
{
    public class GetDepartmentOptionsResponseModel
    {
        public required Guid DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}