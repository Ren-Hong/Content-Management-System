namespace Cms.Contract.Services.Content.Dtos
{
    public class CreateContentRequestDto
    {
        public Guid TypeId { get; set; }

        public Guid OwnerId { get; set; }

        public required List<CreateContentFieldValueDto> FieldValues { get; set; }
    }

    public class CreateContentFieldValueDto
    {
        public Guid FieldId { get; set; }

        public string? FieldValue { get; set; }
    }
}
