using Cms.Contract.Services.Scope.Dtos;

namespace Cms.Contract.Services.Scope.Interfaces
{
    public interface IScopeService
    {
        /// <summary>
        /// 專給下拉用的Scope
        /// </summary>
        /// <returns></returns>
        Task<List<GetScopeOptionsResponseDto>> GetScopeOptionsAsync();
    }
}
