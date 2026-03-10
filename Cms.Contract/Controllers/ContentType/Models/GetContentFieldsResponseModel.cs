namespace Cms.Contract.Controllers.ContentType.Models
{
    public class GetContentFieldsResponseModel
    {
        public Guid FieldId { get; set; }

        public required string FieldName { get; set; }

        public required string FieldCode { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
