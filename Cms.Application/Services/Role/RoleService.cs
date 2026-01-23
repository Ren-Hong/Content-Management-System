using Cms.Application.Services.Account.Dtos;
using Cms.Application.Services.Permission.Dtos;
using Cms.Application.Services.Role.Dtos;
using Cms.Infrastructure.Repositories.Permission;
using Cms.Infrastructure.Repositories.Role;
using Cms.Infrastructure.Repositories.Role.Persistence;
using Cms.Infrastructure.Repositories.UnitOfWork;


namespace Cms.Application.Services.Role
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RoleService(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository
        )
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
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
        public async Task<List<GetRoleSummariesResponseDto>> GetRoleSummariesAsync()
        {
            var rows = (await _roleRepository.GetRoleSummariesAsync()).ToList();

            return rows
                .GroupBy(x => new { x.RoleId, x.RoleName, x.Status }) // 對名稱狀態分組
                .Select(g => new GetRoleSummariesResponseDto
                {
                    RoleId = g.Key.RoleId,
                    RoleName = g.Key.RoleName,
                    Status = (RoleStatus)g.Key.Status, // 從db grouping的資料型態是short, 要自己轉型

                    Permissions = g
                        .Where(x => x.PermissionId.HasValue && x.PermissionName != null)
                        .Select(x => new GetPermissionOptionsResponseDto
                        {
                            PermissionId = x.PermissionId!.Value,
                            PermissionName = x.PermissionName!
                        })
                        .DistinctBy(r => r.PermissionId)
                        .ToList()
                })
                .ToList();
        }

        public async Task<CreateRoleResponseDto> CreateRoleAsync(CreateRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleNameRequired
                };
            }

            // 角色代碼沒給
            if (string.IsNullOrWhiteSpace(dto.RoleCode))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleCodeRequired
                };
            }

            // PermissionIds 沒new 或裡面沒東西
            if (dto.PermissionIds == null || !dto.PermissionIds.Any())
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionIdsRequired
                };
            }

            //每一筆PermissionIds存不存在表中
            if (!await _permissionRepository.PermissionIdsExistAsync(dto.PermissionIds))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionNotFound
                };
            }

            // 角色名稱是否重複
            if (await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleNameDuplicated
                };
            }


            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 先建帳戶
                var RoleId = await _roleRepository.CreateRoleAsync(
                    dto.RoleName,
                    dto.RoleCode
                );

                // 建多筆 RolePermission
                foreach (var roleId in dto.PermissionIds)
                {
                    await _roleRepository.CreateRolePermissionAsync(
                        RoleId,
                        roleId
                    );
                }

                _unitOfWork.Commit();

                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.Success,
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<UpdateRoleResponseDto> UpdateRoleAsync(UpdateRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.RoleNameRequired
                };
            }

            // PermissionIds 沒new 或裡面沒東西
            if (dto.PermissionIds == null || !dto.PermissionIds.Any())
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionIdsRequired
                };
            }

            // 有沒有這角色名稱
            if (!await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.RoleNameNotFound
                };
            }

            //每一筆PermissionIds存不存在表中
            if (!await _permissionRepository.PermissionIdsExistAsync(dto.PermissionIds))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionNotFound
                };
            }

            // 有沒有這種狀態
            if (!Enum.IsDefined(dto.Status))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.StatusNotFound
                };
            }

            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 更新帳戶狀態
                var RoleId = await _roleRepository.UpdateStatusAsync(
                    dto.RoleName,
                    dto.Status
                );

                // 更新帳戶角色（先清掉再加，最乾淨）
                await _roleRepository.DeleteRolePermissionsAsync(RoleId);

                foreach (var permissionId in dto.PermissionIds)
                {
                    await _roleRepository.AddRolePermissionAsync(RoleId, permissionId);
                }

                _unitOfWork.Commit();

                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteRoleResponseDto> DeleteRoleAsync(DeleteRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new DeleteRoleResponseDto
                {
                    Result = DeleteRoleResult.RoleNameRequired
                };
            }

            // 有沒有這角色名稱
            if (!await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new DeleteRoleResponseDto
                {
                    Result = DeleteRoleResult.RoleNotFound
                };
            }

            await _roleRepository.DeleteRoleAsync(dto.RoleName);

            return new DeleteRoleResponseDto
            {
                Result = DeleteRoleResult.Success
            };
        }

    }
}
