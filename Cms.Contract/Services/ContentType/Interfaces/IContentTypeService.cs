using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.ContentType.Dtos;

namespace Cms.Contract.Services.ContentType.Interfaces
{
    public interface IContentTypeService
    {
        Task<List<GetContentTypeOptionsResponseDto>> GetContentTypeOptionsAsync(GetContentTypeOptionsRequestDto req);

        Task<List<GetContentFieldsResponseDto>> GetContentFieldsAsync(Guid typeId);

        Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req);

        Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req);

        Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req);
    }
}
