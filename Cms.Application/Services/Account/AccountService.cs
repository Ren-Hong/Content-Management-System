using Cms.Application.Services.Account.Dtos;
using Cms.Application.Services.Domain;
using Cms.Infrastructure.Repositories.Account;
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

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var rows = (await _accountRepository.GetAccountAuthByUsernameAsync(request.Username)).ToList();

            if (!rows.Any())
                return new LoginResponseDto
                {
                    Result = LoginResult.AccountNotFound
                };

            var auth = new GetAccountAuthResponseDto
            {
                AccountId = rows[0].AccountId,
                Username = rows[0].Username,
                PasswordHash = rows[0].PasswordHash,
                Roles = rows
                    .Where(x => x.RoleCode != null)
                    .Select(x => x.RoleCode!)
                    .Distinct()
                    .ToArray(),
                Permissions = rows
                    .Where(x => x.PermissionCode != null)
                    .Select(x => x.PermissionCode!)
                    .Distinct()
                    .ToArray()
            };

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(request.Password, auth.PasswordHash))
                return new LoginResponseDto
                {
                    Result = LoginResult.InvalidPassword
                };

            await _accountRepository.UpdateLastLoginAtAsync(auth.AccountId, DateTime.UtcNow);

            return new LoginResponseDto
            {
                Result = LoginResult.Success,
                AccountId = auth.AccountId,
                Username = auth.Username,
                Roles = auth.Roles,
                Permissions = auth.Permissions
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

            // RoleCode 沒給
            if (string.IsNullOrWhiteSpace(dto.RoleCode))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleRequired
                };
            }

            // 有沒有這角色
            if (!await _roleRepository.RoleExistsAsync(dto.RoleCode))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleNotExist
                };
            }

            // 帳號是否重複
            if (await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.UsernameAlreadyExists
                };
            }

            // 灑鹽加密
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);


            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // RoleCode 轉 RoleId
                var roleId = await _roleRepository.GetRoleIdByCodeAsync(dto.RoleCode);

                // 判斷有沒有這個RoleId
                if (roleId == null)
                {
                    _unitOfWork.Rollback();
                    return new CreateAccountResponseDto
                    {
                        Result = CreateAccountResult.RoleNotExist
                    };
                }

                // 先建帳戶
                var accountId = await _accountRepository.CreateAccountAsync(
                    dto.Username,
                    passwordHash
                );

                // 再建角色
                await _accountRepository.CreateAccountRoleAsync(
                    accountId,
                    roleId.Value
                );

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

            // RoleCode沒給
            if (string.IsNullOrWhiteSpace(dto.RoleCode))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleCodeRequired
                };
            }

            // 有沒有這帳號
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.AccountNotExist
                };
            }

            // 有沒有這角色
            if (!await _roleRepository.RoleExistsAsync(dto.RoleCode))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleNotExist
                };
            }

            // 有沒有這種狀態
            if (!Enum.IsDefined(dto.Status))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.StatusNotExist
                };
            }

            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 更新帳戶狀態
                var accountId = await _accountRepository.UpdateAccountStatusAsync(
                    dto.Username,
                    (short)dto.Status
                );

                // RoleCode 轉 RoleId
                var roleId = await _roleRepository.GetRoleIdByCodeAsync(dto.RoleCode);

                // 更新帳戶角色（先清掉再加，最乾淨）
                await _accountRepository.UpdateAccountRoleAsync(
                    accountId,
                    roleId.Value
                );

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
    }
}
