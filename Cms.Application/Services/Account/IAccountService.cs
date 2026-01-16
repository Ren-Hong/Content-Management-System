using Cms.Application.Services.Account.Dtos;

namespace Cms.Application.Services.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns></returns>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
