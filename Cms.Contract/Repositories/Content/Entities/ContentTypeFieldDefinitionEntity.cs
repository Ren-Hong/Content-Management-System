namespace Cms.Contract.Repositories.Content.Entities
{
    public class ContentTypeFieldDefinitionEntity
    {
        public Guid FieldId { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
