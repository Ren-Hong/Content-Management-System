namespace Cms.Contract.Repositories.ContentType.Entities
{
    public class ContentTypeOptionEntity
    {
        public required Guid TypeId { get; set; }

        public required string TypeName { get; set; }
    }
}