namespace Cms.Contract.Services.Content.Dtos
{
    public class DeleteContentResponseDto
    {
        public DeleteContentResult Result { get; set; }
    }

    public enum DeleteContentResult
    {
        Success,
        ContentNotFound,
        PermissionDenied
    }
}
