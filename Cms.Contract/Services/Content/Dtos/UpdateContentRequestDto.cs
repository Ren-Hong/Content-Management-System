namespace Cms.Contract.Services.Content.Dtos
{
    public class UpdateContentRequestDto
    {
        public Guid ContentId { get; set; }

        public Guid RevisionId { get; set; }

        public required List<UpdateContentFieldValueDto> FieldValues { get; set; }
    }

    public class UpdateContentFieldValueDto
    {
        public Guid FieldId { get; set; }

        public string? FieldValue { get; set; }
    }
}
