namespace Cms.Contract.Controllers.ContentType.Models
{
    public class GetContentTypeOptionsResponseModel
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}