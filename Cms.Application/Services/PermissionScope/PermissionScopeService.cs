using Cms.Contract.Repositories.Account.Interfaces;
using Cms.Contract.Services.PermissionScope.Interfaces;

namespace Cms.Application.Services.PermissionScope
{
    public class PermissionScopeService : IPermissionScopeService
    {
        private readonly IAccountRepository _accountRepository;

        public PermissionScopeService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> HasGlobalScopeAsync(Guid accountId, string permissionCode)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            return await _accountRepository.HasGlobalPermissionScopeAsync(accountId, permissionCode);
        }

        public async Task<List<Guid>> GetDepartmentScopeDepartmentIdsAsync(Guid accountId, string permissionCode)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(permissionCode))
            {
                return [];
            }

            return await _accountRepository.GetDepartmentIdsByPermissionScopeAsync(accountId, permissionCode);
        }

        public async Task<bool> CanAccessDepartmentAsync(Guid accountId, string permissionCode, Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                return false;
            }

            if (await HasGlobalScopeAsync(accountId, permissionCode))
            {
                return true;
            }

            var departmentIds = await GetDepartmentScopeDepartmentIdsAsync(accountId, permissionCode);

            return departmentIds.Contains(departmentId);
        }
    }
}
