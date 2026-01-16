using Cms.Infrastructure.Repositories.Account;
using Cms.Application.Services.Account.Dtos;
using System.Data;

namespace Cms.Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(
            IAccountRepository accountRepository
        )
        {
            _accountRepository = accountRepository;
        }


        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var rows = (await _accountRepository.GetAccountAuthByUsernameAsync(request.Username)).ToList();

            if (!rows.Any())
                return new LoginResponseDto
                {
                    Result = LoginResult.AccountNotFound
                };

            var auth = new AccountAuthDto
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

            if (!VerifyPassword(request.Password, auth.PasswordHash))
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

        private bool VerifyPassword(string password, string passwordHash)
        {
            // 先簡單版，之後可以換 BCrypt / PBKDF2
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
