using Cms.Contract.Repositories.Scope.Entities;

namespace Cms.Contract.Repositories.Scope.Interfaces
{
    public interface IScopeRepository
    {
        /// <summary>
        /// 給下拉用的scope
        /// </summary>
        /// <param name="includeAssigned">預設不給assign</param>
        /// <returns></returns>
        Task<IEnumerable<ScopeOptionEntity>> GetScopeOptionsAsync(bool includeAssigned = false);

        /// <summary>
        /// 判斷該作用範圍是否存在
        /// </summary>
        /// <returns>bool</returns>
        Task<bool> AllScopesExistAsync(List<Guid> scopeIds);
    }
}
