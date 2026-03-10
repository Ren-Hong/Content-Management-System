using Cms.Contract.Services.Content.Dtos;

namespace Cms.Contract.Services.Content.Interfaces
{
    public interface IContentService
    {
        Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto);
    }
}
