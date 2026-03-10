namespace Cms.Contract.Services.ContentType.Dtos
{
    public class CreateContentTypeResponseDto
    {
        public CreateContentTypeResult Result { get; set; }

        public Guid? TypeId { get; set; }

        public string? TypeName { get; set; }
    }

    public enum CreateContentTypeResult
    {
        Success,
        DepartmentRequired,
        TypeNameRequired,
        TypeNameDuplicated,
        FieldRequired,
        FieldNameRequired,
        FieldNameDuplicated,
        FieldTypeRequired,
        FieldTypeInvalid
    }
}
