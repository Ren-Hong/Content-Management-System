namespace Cms.Infrastructure.Repositories.Account.Persistence
{
    public enum AccountStatus : short
    {
        Enable   = 1, // 啟用
        Disabled = 2, // 軟刪
        Locked   = 3, // 鎖帳
    }
}
