using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.PermissionScope.Interfaces;
using Cms.Contract.Services.Department.Dtos;
using Cms.Contract.Services.Department.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private const string ContentViewPermissionCode = "Content.View";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPermissionScopeService _permissionScopeService;

        public DepartmentService(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepository,
            IPermissionScopeService permissionScopeService
        )
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
            _permissionScopeService = permissionScopeService;
        }

        public async Task<List<GetDepartmentOptionsResponseDto>> GetDepartmentOptionsAsync()
        {
            var rows = await _departmentRepository.GetDepartmentOptionsAsync();

            return rows
                .Select(x => new GetDepartmentOptionsResponseDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }

        public async Task<List<GetDepartmentsForSidebarResponseDto>> GetDepartmentsForSidebarAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return [];
            }

            var hasGlobalScope = await _permissionScopeService.HasGlobalScopeAsync(
                accountId,
                ContentViewPermissionCode
            );

            var rows = hasGlobalScope
                ? await _departmentRepository.GetDepartmentsForSidebarAsync()
                : await GetDepartmentScopedRowsAsync(accountId);

            return rows
                .Select(x => new GetDepartmentsForSidebarResponseDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }

        private async Task<IEnumerable<Cms.Contract.Repositories.Department.Entities.DepartmentSidebarEntity>> GetDepartmentScopedRowsAsync(Guid accountId)
        {
            var departmentIds = await _permissionScopeService.GetDepartmentScopeDepartmentIdsAsync(
                accountId,
                ContentViewPermissionCode
            );

            if (departmentIds.Count == 0)
            {
                return [];
            }

            return await _departmentRepository.GetDepartmentsForSidebarAsync(departmentIds);
        }
    }
}
