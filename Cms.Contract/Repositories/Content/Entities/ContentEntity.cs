namespace Cms.Contract.Repositories.Content.Entities
{
    public class ContentEntity
    {
        public Guid ContentId { get; set; }

        public Guid RevisionId { get; set; }

        public int Version { get; set; }

        public Guid OwnerId { get; set; }

        public required string OwnerUsername { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid FieldId { get; set; }

        public required string FieldCode { get; set; }

        public required string FieldType { get; set; }

        public string? FieldValue { get; set; }

        public int SortOrder { get; set; }
    }
}
