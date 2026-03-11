namespace Cms.Contract.Services.Content.Dtos
{
    public class GetContentsResponseDto
    {
        public Guid ContentId { get; set; }

        public Guid RevisionId { get; set; }

        public int Version { get; set; }

        public required string OwnerUsername { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public required List<GetContentFieldValueDto> FieldValues { get; set; }
    }

    public class GetContentFieldValueDto
    {
        public Guid FieldId { get; set; }

        public required string FieldName { get; set; }

        public required string FieldType { get; set; }

        public string? FieldValue { get; set; }
    }
}
