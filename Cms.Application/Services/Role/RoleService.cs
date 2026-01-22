using Cms.Infrastructure.Repositories.Role;
using Cms.Infrastructure.Repositories.UnitOfWork;
using Cms.Application.Services.Role.Dtos;


namespace Cms.Application.Services.Role
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public RoleService(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository
        )
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<List<GetRoleOptionsResponseDto>> GetRoleOptionsAsync()
        {
            var rows = await _roleRepository.GetRoleOptionsAsync();

            return rows
                .Select(x => new GetRoleOptionsResponseDto
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName
                })
                .ToList();
        }
    }
}
