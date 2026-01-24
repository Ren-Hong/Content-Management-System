using System.Data;

namespace Cms.Contract.Services.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 目前作用中的 Transaction（沒有交易時為 null）
        /// Repository 只讀這個，不負責建立或釋放
        /// </summary>
        IDbTransaction? Transaction { get; }

        /// <summary>
        /// 開始一個交易
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 提交交易（內部一定會 Dispose）
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滾交易（內部一定會 Dispose）
        /// </summary>
        void Rollback();
    }
}
