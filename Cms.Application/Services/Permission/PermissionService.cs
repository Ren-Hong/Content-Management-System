using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Services.Permission.Dtos;
using Cms.Contract.Services.Permission.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Permission
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(
            IUnitOfWork unitOfWork,
            IPermissionRepository permissionRepository
        )
        {
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<GetPermissionOptionsResponseDto>> GetPermissionOptionsAsync()
        {
            var rows = await _permissionRepository.GetPermissionOptionsAsync();

            return rows
                .Select(x => new GetPermissionOptionsResponseDto
                {
                    PermissionId = x.PermissionId,
                    PermissionName = x.PermissionName
                })
                .ToList();
        }
    }
}
