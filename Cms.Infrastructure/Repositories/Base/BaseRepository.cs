using Cms.Infrastructure.Repositories.UnitOfWork;
using System.Data;

namespace Cms.Infrastructure.Repositories.Base
{
    public abstract class BaseRepository
    {
        protected readonly IDbConnection _db;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        )
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        protected IDbTransaction? Tx => _unitOfWork.Transaction; // 如果Service有跑BeginTransaction 這個屬性就會被附值
    }
}
