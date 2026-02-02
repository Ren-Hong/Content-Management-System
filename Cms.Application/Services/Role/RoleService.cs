using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Role.Persistence;
using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Services.Permission.Dtos;
using Cms.Contract.Services.Role.Dtos;
using Cms.Contract.Services.Role.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Role
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IScopeRepository _scopeRepository;


        public RoleService
        (
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IScopeRepository scopeRepository
        )
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _scopeRepository = scopeRepository;
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
                .GroupBy(r => new { r.RoleId, r.RoleName, r.Status })
                .Select(roleGroup => new GetRoleSummariesResponseDto
                {
                    RoleId = roleGroup.Key.RoleId,
                    RoleName = roleGroup.Key.RoleName,
                    Status = roleGroup.Key.Status,

                    PermissionScopes = roleGroup
                        .GroupBy(x => new { x.PermissionId, x.PermissionName })
                        .Select(pGroup => new PermissionScopesSummaryDto
                        {
                            PermissionId = pGroup.Key.PermissionId!.Value,
                            PermissionName = pGroup.Key.PermissionName!,

                            Scopes = pGroup
                                .Select(s => new ScopeSummaryDto
                                {
                                    ScopeId = s.ScopeId!.Value,
                                    ScopeName = s.ScopeName!
                                })
                                .DistinctBy(s => s.ScopeId)
                                .ToList()
                        })
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

            // PermissionScopes 沒給
            if (dto.PermissionScopes == null || !dto.PermissionScopes.Any())
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionRequired
                };
            }

            var normalizedScopes = dto.PermissionScopes
                .DistinctBy(x => new { x.PermissionId, x.ScopeId })
                .ToList();

            var permissionIds = normalizedScopes
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            var scopeIds = normalizedScopes
                .Select(x => x.ScopeId)
                .Distinct()
                .ToList();

            // Permission 是否存在
            if (!await _permissionRepository.AllPermissionsExistAsync(permissionIds))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionNotFound
                };
            }

            // Scope 是否存在
            if (!await _scopeRepository.AllScopesExistAsync(scopeIds))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.ScopeNotFound
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
                // 建立 Role
                var roleId = await _roleRepository.CreateRoleAsync(
                    dto.RoleName,
                    dto.RoleCode
                );

                // 建立 RolePermission（能力層）
                foreach (var permissionId in permissionIds)
                {
                    await _roleRepository.CreateRolePermissionAsync(
                        roleId,
                        permissionId
                    );
                }

                // 建立 RolePermissionScopes（範圍層）
                foreach (var item in dto.PermissionScopes)
                {
                    await _roleRepository.CreateRolePermissionScopeAsync(
                        roleId,
                        item.PermissionId,
                        item.ScopeId
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

            // PermissionScopes 沒給
            if (dto.PermissionScopes == null || !dto.PermissionScopes.Any())
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionRequired
                };
            }

            var normalizedScopes = dto.PermissionScopes
                .DistinctBy(x => new { x.PermissionId, x.ScopeId })
                .ToList();

            var permissionIds = normalizedScopes
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            var scopeIds = normalizedScopes
                .Select(x => x.ScopeId)
                .Distinct()
                .ToList();

            // Permission 是否存在
            if (!await _permissionRepository.AllPermissionsExistAsync(permissionIds))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionNotFound
                };
            }

            // Scope 是否存在
            if (!await _scopeRepository.AllScopesExistAsync(scopeIds))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.ScopeNotFound
                };
            }

            // 角色名稱是否重複
            if (!await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.RoleNameNotFound
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
                var roleId = await _roleRepository.UpdateStatusAsync(
                    dto.RoleName,
                    dto.Status
                );

                // 更新帳戶角色（先清掉再加，最乾淨）, SQL 有下 DELETE CASCADE 不用額外去刪Scope
                await _roleRepository.DeleteRolePermissionsAsync(roleId);

                foreach (var permissionId in permissionIds)
                {
                    await _roleRepository.CreateRolePermissionAsync(
                        roleId, 
                        permissionId
                    );
                }

                // 建立 RolePermissionScopes（範圍層）
                foreach (var item in dto.PermissionScopes)
                {
                    await _roleRepository.CreateRolePermissionScopeAsync(
                        roleId,
                        item.PermissionId,
                        item.ScopeId
                    );
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
