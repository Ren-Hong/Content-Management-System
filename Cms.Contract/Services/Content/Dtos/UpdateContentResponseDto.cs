namespace Cms.Contract.Services.Content.Dtos
{
    public class UpdateContentResponseDto
    {
        public UpdateContentResult Result { get; set; }
    }

    public enum UpdateContentResult
    {
        Success,
        ContentNotFound,
        PermissionDenied,
        RevisionNotFound,
        FieldValueRequired,
        FieldNotFound,
        RequiredFieldMissing
    }
}
