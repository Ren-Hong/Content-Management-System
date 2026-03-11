namespace Cms.Contract.Services.PermissionScope.Interfaces
{
    public interface IPermissionScopeService
    {
        Task<bool> HasGlobalScopeAsync(Guid accountId, string permissionCode);

        Task<List<Guid>> GetDepartmentScopeDepartmentIdsAsync(Guid accountId, string permissionCode);

        Task<bool> CanAccessDepartmentAsync(Guid accountId, string permissionCode, Guid departmentId);
    }
}
