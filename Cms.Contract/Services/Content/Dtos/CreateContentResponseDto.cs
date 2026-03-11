namespace Cms.Contract.Services.Content.Dtos
{
    public class CreateContentResponseDto
    {
        public CreateContentResult Result { get; set; }

        public Guid? ContentId { get; set; }

        public Guid? RevisionId { get; set; }
    }

    public enum CreateContentResult
    {
        Success,
        TypeNotFound,
        PermissionDenied,
        OwnerRequired,
        FieldValueRequired,
        FieldNotFound,
        RequiredFieldMissing
    }
}
