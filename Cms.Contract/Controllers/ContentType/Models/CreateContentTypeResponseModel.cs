namespace Cms.Contract.Controllers.ContentType.Models
{
    public class CreateContentTypeResponseModel
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}
