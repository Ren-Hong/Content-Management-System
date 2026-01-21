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
        Task<LoginResponseDto> LoginAsync(LoginRequestDto req);

        /// <summary>
        /// 取得所有帳戶摘要
        /// </summary>
        /// <returns></returns>
        Task<List<GetAccountSummariesResponseDto>> GetAccountSummariesAsync();

        /// <summary>
        /// 建立帳戶
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto req);

        /// <summary>
        /// 更新帳戶
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<UpdateAccountResponseDto> UpdateAccountAsync(UpdateAccountRequestDto req);

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto req);

        /// <summary>
        /// 軟刪帳戶
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto req);
    }
}
