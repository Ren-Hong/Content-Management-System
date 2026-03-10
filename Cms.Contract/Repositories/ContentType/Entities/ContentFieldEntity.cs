namespace Cms.Contract.Repositories.ContentType.Entities
{
    public class ContentFieldEntity
    {
        public Guid FieldId { get; set; }

        public required string FieldCode { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
