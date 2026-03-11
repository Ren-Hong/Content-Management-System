namespace Cms.Contract.Controllers.Content.Models
{
    public class GetContentsResponseModel
    {
        public Guid ContentId { get; set; }

        public Guid RevisionId { get; set; }

        public int Version { get; set; }

        public required string OwnerUsername { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public required List<GetContentFieldValueModel> FieldValues { get; set; }
    }

    public class GetContentFieldValueModel
    {
        public Guid FieldId { get; set; }

        public required string FieldName { get; set; }

        public required string FieldType { get; set; }

        public string? FieldValue { get; set; }
    }
}
