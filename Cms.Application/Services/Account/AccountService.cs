using Cms.Application.Services.Account.Dtos;
using Cms.Infrastructure.Repositories.Account;
using Cms.Infrastructure.Repositories.Account.Persistence;
using Cms.Infrastructure.Repositories.Role;
using Cms.Infrastructure.Repositories.UnitOfWork;
using System.Data;


namespace Cms.Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;

        public AccountService(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository
        )
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
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

        public async Task<List<GetAccountSummariesResponseDto>> GetAccountSummariesAsync()
        {
            var rows = (await _accountRepository.GetAccountSummariesAsync()).ToList();

            return rows
                .GroupBy(x => new { x.Username, x.Status }) // 對名稱狀態分組
                .Select(g => new GetAccountSummariesResponseDto
                {
                    Username = g.Key.Username,
                    Status = (AccountStatus)g.Key.Status, // 從db grouping的資料型態是short, 要自己轉型
                    Roles = g
                        .Where(x => !string.IsNullOrEmpty(x.RoleCode)) // 角色代碼不能為空
                        .Select(x => new RoleResponseDto 
                        {
                            RoleCode = x.RoleCode,
                            RoleName = x.RoleName
                        }) 
                        .DistinctBy(r => r.RoleCode) // ⭐ 關鍵：用 RoleCode 去重
                        .ToList()
                })
                .ToList();
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

            // RoleCodes 沒new 或裡面沒東西
            if (dto.RoleCodes == null || !dto.RoleCodes.Any())
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleCodesRequired
                };
            }

            // 有沒有這角色 (一次驗)
            var roleIds = (await _roleRepository.GetRoleIdsByRoleCodesAsync(dto.RoleCodes)).ToList();

            if (roleIds.Count != dto.RoleCodes.Count)
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleNotFound
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

                // 建多筆 AccountRole
                foreach (var roleId in roleIds)
                {
                    await _accountRepository.CreateAccountRoleAsync(
                        accountId, 
                        roleId
                    );
                }

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

            // RoleCodes 沒new 或裡面沒東西
            if (dto.RoleCodes == null || !dto.RoleCodes.Any())
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleCodesRequired
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

            // 有沒有這角色 (一次驗)
            var roleIds = (await _roleRepository.GetRoleIdsByRoleCodesAsync(dto.RoleCodes)).ToList();

            if (roleIds.Count != dto.RoleCodes.Count)
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleNotFound
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

                foreach (var roleId in roleIds)
                {
                    await _accountRepository.AddAccountRoleAsync(accountId, roleId);
                }

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
