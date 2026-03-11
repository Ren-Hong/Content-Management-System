using Cms.Contract.Services.Content.Dtos;

namespace Cms.Contract.Services.Content.Interfaces
{
    public interface IContentService
    {
        Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto);

        Task<List<GetContentsResponseDto>> GetContentsAsync(Guid typeId, Guid accountId);

        Task<UpdateContentResponseDto> UpdateContentAsync(UpdateContentRequestDto requestDto, Guid accountId);

        Task<DeleteContentResponseDto> DeleteContentAsync(DeleteContentRequestDto requestDto, Guid accountId);
    }
}
