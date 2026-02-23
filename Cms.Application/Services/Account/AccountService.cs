using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Account.Interfaces;
using Cms.Contract.Repositories.Account.Persistence;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.Account.Dtos;
using Cms.Contract.Services.Account.Interfaces;
using Cms.Contract.Services.Department.Dtos;
using Cms.Contract.Services.Role.Dtos;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using System.Data;


namespace Cms.Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public AccountService(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new LoginResponseDto
                {
                    Result = LoginResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new LoginResponseDto
                {
                    Result = LoginResult.PasswordRequired
                };
            }

            // 取得帳號角色權限
            var rows = (await _accountRepository.GetAccountAuthByUsernameAsync(dto.Username)).ToList();

            // 找不到帳號
            if (!rows.Any())
                return new LoginResponseDto
                {
                    Result = LoginResult.AccountNotFound
                };

            // Entity 轉 Dto
            var auth = new GetAccountAuthResponseDto 
            {
                
                AccountId = rows[0].AccountId,
                Username = rows[0].Username,
                PasswordHash = rows[0].PasswordHash,

                RoleCodes = rows
                    .Select(x => x.RoleCode)                            // 只拿 RoleCodes
                    .Where(code => !string.IsNullOrWhiteSpace(code))    // 去 Null 與 空字串, IEnumerable<string?>
                    .Select(code => code!)                              // IEnumerable<string?> -> IEnumerable<string>
                    .Distinct()
                    .ToList(),

                PermissionCodes = rows
                    .Select(x => x.PermissionCode)                            
                    .Where(code => !string.IsNullOrWhiteSpace(code))    
                    .Select(code => code!)                              
                    .Distinct()
                    .ToList(),
            };

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, auth.PasswordHash))
                return new LoginResponseDto
                {
                    Result = LoginResult.InvalidPassword
                };

            // 更新上一次登入時間
            await _accountRepository.UpdateLastLoginAtAsync(auth.AccountId, DateTime.UtcNow);

            return new LoginResponseDto
            {
                Result = LoginResult.Success,
                AccountId = auth.AccountId,
                Username = auth.Username,
                RoleCodes = auth.RoleCodes,
                PermissionCodes = auth.PermissionCodes
            };
        }

        public async Task<PagedResult<GetAccountSummariesResponseDto>> GetAccountSummariesAsync(PageRequest preq)
        {
            var pagedRows = await _accountRepository.GetAccountSummariesPagedAsync(preq);

            var grouped = pagedRows.Items
                .GroupBy(x => new { x.AccountId, x.Username, x.Status })
                .Select(g => new GetAccountSummariesResponseDto
                {
                    AccountId = g.Key.AccountId,
                    Username  = g.Key.Username,
                    Status    = (AccountStatus)g.Key.Status,

                    RoleAssignments = g
                        .Where(r => r.RoleId.HasValue && r.RoleName != null)
                        .GroupBy(r => new { RoleId = r.RoleId!.Value, RoleName = r.RoleName! })
                        .Select(roleGroup => new AccountRoleAssignmentResponseDto
                        {
                            RoleId = roleGroup.Key.RoleId,
                            RoleName = roleGroup.Key.RoleName,

                            Departments = roleGroup
                                .Where(r => r.DepartmentId.HasValue && r.DepartmentName != null)
                                .GroupBy(r => new { DeptId = r.DepartmentId!.Value, DeptName = r.DepartmentName! })
                                .Select(dg => new GetDepartmentOptionsResponseDto
                                {
                                    DepartmentId = dg.Key.DeptId,
                                    DepartmentName = dg.Key.DeptName
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return new PagedResult<GetAccountSummariesResponseDto>
            {
                Page = pagedRows.Page,
                PageSize = pagedRows.PageSize,
                TotalCount = pagedRows.TotalCount,
                Items = grouped
            };
        }

        public async Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.PasswordRequired
                };
            }

            // RoleAssignments 沒給
            if (dto.RoleAssignments == null || !dto.RoleAssignments.Any())
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleAssignmentsRequired
                };
            }

            // 檢查 Role 是否存在
            var roleIds = dto.RoleAssignments
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            if (!await _roleRepository.AllRolesExistAsync(roleIds))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleNotFound
                };
            }

            // 檢查 Department 是否存在
            var departmentIds = dto.RoleAssignments
                .SelectMany(x => x.DepartmentIds) // 因為一個 RoleAssignment 可能對多個 Department, 所以要 SelectMany 展平
                .Distinct()
                .ToList();

            if (!await _departmentRepository.AllDepartmentsExistAsync(departmentIds))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.DepartmentNotFound
                };
            }

            // 帳號是否重複
            if (await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.UsernameDuplicated
                };
            }

            // 灑鹽加密
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);


            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 先建帳戶
                var accountId = await _accountRepository.CreateAccountAsync(
                    dto.Username,
                    passwordHash
                );

                // 建多筆 AccountRoleAssignment
                var rows = dto.RoleAssignments
                    .SelectMany(r => r.DepartmentIds.Select(d => new //心法：SelectMany(RoleId) + Select(DepartmentId) 就是雙層foreach
                    {
                        AccountId = accountId,
                        RoleId = r.RoleId,
                        DepartmentId = d
                    }))
                    .ToList();

                await _accountRepository.CreateAccountRoleAssignmentsAsync(rows);

                _unitOfWork.Commit();

                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.Success,
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<UpdateAccountResponseDto> UpdateAccountAsync(UpdateAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.UsernameRequired
                };
            }

            // RoleAssignments 沒給
            if (dto.RoleAssignments == null || !dto.RoleAssignments.Any())
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleAssignmentsRequired
                };
            }

            // 檢查 Role 是否存在
            var roleIds = dto.RoleAssignments
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            if (!await _roleRepository.AllRolesExistAsync(roleIds))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleNotFound
                };
            }

            // 檢查 Department 是否存在
            var departmentIds = dto.RoleAssignments
                .SelectMany(x => x.DepartmentIds)
                .Distinct()
                .ToList();

            if (!await _departmentRepository.AllDepartmentsExistAsync(departmentIds))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.DepartmentNotFound
                };
            }

            // 有沒有這帳號
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.AccountNotFound
                };
            }

            // 有沒有這種狀態
            if (!Enum.IsDefined(dto.Status))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.StatusNotFound
                };
            }

            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 更新帳戶狀態
                var accountId = await _accountRepository.UpdateStatusAsync(
                    dto.Username,
                    dto.Status
                );

                // 更新帳戶角色（先清掉再加，最乾淨）
                await _accountRepository.DeleteAccountRolesAsync(accountId);

                // 建多筆 AccountRoleAssignment
                var rows = dto.RoleAssignments
                    .SelectMany(r => r.DepartmentIds.Select(d => new
                    {
                        AccountId = accountId,
                        RoleId = r.RoleId,
                        DepartmentId = d
                    }))
                    .ToList();

                await _accountRepository.CreateAccountRoleAssignmentsAsync(rows);

                _unitOfWork.Commit();

                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.PasswordRequired
                };
            }

            // 帳號不存在
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.AccountNotFound
                };
            }

            // 加鹽撒密
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 更新密碼
            await _accountRepository.UpdatePasswordAsync(dto.Username, passwordHash);

            // 更新上次更新時間
            await _accountRepository.UpdateUpdatedAtAsync(dto.Username, DateTime.Now);

            return new ResetPasswordResponseDto
            {
                Result = ResetPasswordResult.Success
            };
        }

        public async Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new DeleteAccountResponseDto
                {
                    Result = DeleteAccountResult.UsernameRequired
                };
            }

            // 有沒有這帳號
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new DeleteAccountResponseDto
                {
                    Result = DeleteAccountResult.AccountNotFound
                };
            }

            await _accountRepository.DeleteAccountAsync(dto.Username);

            return new DeleteAccountResponseDto
            {
                Result = DeleteAccountResult.Success
            };
        }
    }
}
