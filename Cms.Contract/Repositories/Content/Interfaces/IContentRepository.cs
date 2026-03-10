using Cms.Contract.Repositories.Content.Entities;

namespace Cms.Contract.Repositories.Content.Interfaces
{
    public interface IContentRepository
    {
        Task<bool> ContentTypeExistsAsync(Guid typeId);

        Task<List<ContentTypeFieldDefinitionEntity>> GetFieldDefinitionsByTypeIdAsync(Guid typeId);

        Task<Guid> CreateContentAsync(Guid typeId, Guid ownerId, string status);

        Task<Guid> CreateContentRevisionAsync(Guid contentId, int version);

        Task CreateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue);
    }
}
