using Cms.Contract.Repositories.Content.Entities;

namespace Cms.Contract.Repositories.Content.Interfaces
{
    public interface IContentRepository
    {
        Task<bool> ContentTypeExistsAsync(Guid typeId);

        Task<bool> ContentExistsAsync(Guid contentId);

        Task<bool> ContentRevisionExistsAsync(Guid revisionId);

        Task<bool> ContentRevisionBelongsToContentAsync(Guid contentId, Guid revisionId);

        Task<Guid?> GetTypeIdByRevisionIdAsync(Guid revisionId);

        Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId);

        Task<Guid?> GetDepartmentIdByContentIdAsync(Guid contentId);

        Task<List<ContentTypeFieldDefinitionEntity>> GetFieldDefinitionsByTypeIdAsync(Guid typeId);

        Task<List<ContentEntity>> GetContentsAsync(Guid typeId);

        Task<Guid> CreateContentAsync(Guid typeId, Guid ownerId, string status);

        Task<Guid> CreateContentRevisionAsync(Guid contentId, int version);

        Task CreateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue);

        Task UpdateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue);

        Task DeleteContentFieldValuesByContentIdAsync(Guid contentId);

        Task DeleteContentRevisionsByContentIdAsync(Guid contentId);

        Task DeleteContentAsync(Guid contentId);
    }
}
