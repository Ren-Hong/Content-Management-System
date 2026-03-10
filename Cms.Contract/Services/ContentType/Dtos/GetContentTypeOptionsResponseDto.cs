namespace Cms.Contract.Services.ContentType.Dtos
{
    public class GetContentTypeOptionsResponseDto
    {
        public Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}