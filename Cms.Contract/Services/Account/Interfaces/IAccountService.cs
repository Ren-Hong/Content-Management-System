using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.Account.Dtos;

namespace Cms.Contract.Services.Account.Interfaces
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
        Task<PagedResult<GetAccountSummariesResponseDto>> GetAccountSummariesAsync(PageRequest preq);

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
        /// 真實刪除帳戶
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto req);
    }
}
