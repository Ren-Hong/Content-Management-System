namespace Cms.Contract.Services.ContentType.Dtos
{
    public class GetContentFieldsResponseDto
    {
        public Guid FieldId { get; set; }

        public required string FieldName { get; set; }

        public required string FieldCode { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
