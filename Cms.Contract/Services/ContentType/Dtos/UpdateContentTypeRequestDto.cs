namespace Cms.Contract.Services.ContentType.Dtos
{
    public class UpdateContentTypeRequestDto
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }

        public required List<UpdateContentTypeFieldDto> Fields { get; set; }
    }

    public class UpdateContentTypeFieldDto
    {
        public Guid? FieldId { get; set; }

        public required string FieldName { get; set; }

        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
