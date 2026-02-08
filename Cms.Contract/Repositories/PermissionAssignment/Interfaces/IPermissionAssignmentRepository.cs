using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.PermissionAssignment.Entities;

namespace Cms.Contract.Repositories.PermissionAssignment.Interfaces
{
    public interface IPermissionAssignmentRepository
    {
        /// <summary>
        /// 拿特殊指派摘要
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<PermissionAssignmentSummaryEntity>> GetPermissionAssignmentSummariesPagedAsync(PageRequest preq);

    }
}
