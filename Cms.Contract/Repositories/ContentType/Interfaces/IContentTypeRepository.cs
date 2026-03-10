using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.ContentType.Entities;

namespace Cms.Contract.Repositories.ContentType.Interfaces
{
    public interface IContentTypeRepository
    {
        Task<IEnumerable<ContentTypeOptionEntity>> GetContentTypeOptionsAsync(Guid departmentId);

        Task<IEnumerable<ContentFieldEntity>> GetContentFieldsAsync(Guid typeId);

        Task<bool> TypeNameExistsAsync(Guid departmentId, string typeName);

        Task<Guid?> GetTypeIdByDepartmentIdAndTypeNameAsync(Guid departmentId, string typeName);

        Task<bool> TypeExistsAsync(Guid typeId);

        Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId);

        Task<Guid> CreateContentTypeAsync(Guid departmentId, string typeName, string typeCode);

        Task<Guid> CreateContentFieldAsync(string fieldCode, string fieldType, bool isRequired);

        Task CreateContentTypeFieldAsync(Guid typeId, Guid fieldId, int sortOrder);

        Task UpdateContentTypeAsync(Guid typeId, string typeName);

        Task UpdateContentFieldAsync(Guid fieldId, string fieldCode, string fieldType, bool isRequired);

        Task UpdateContentTypeFieldSortOrderAsync(Guid typeId, Guid fieldId, int sortOrder);

        Task DeleteContentTypeFieldAsync(Guid typeId, Guid fieldId);

        Task DeleteContentFieldAsync(Guid fieldId);

        Task DeleteContentTypeAsync(Guid typeId);

        Task<bool> ContentExistsByTypeIdAsync(Guid typeId);

        Task<bool> ContentFieldValueExistsAsync(Guid fieldId);
    }
}
