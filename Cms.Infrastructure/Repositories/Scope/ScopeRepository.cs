using Cms.Contract.Repositories.Role.Entities;
using Cms.Contract.Repositories.Scope.Entities;
using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;


namespace Cms.Infrastructure.Repositories.Scope
{
    public class ScopeRepository : BaseRepository, IScopeRepository
    {
        public ScopeRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<ScopeOptionEntity>> GetScopeOptionsAsync(bool includeAssigned = false)
        {
            const string sql = @"
                SELECT
                    ScopeId,
                    ScopeCode,
                    ScopeName
                FROM Scopes
                WHERE (@IncludeAssigned = 1 OR ScopeCode <> 'Assigned')
                ORDER BY ScopeName
            ";

            return await _db.QueryAsync<ScopeOptionEntity>(
                sql,
                new { IncludeAssigned = includeAssigned ? 1 : 0 },
                transaction: Tx
            );
        }

        public async Task<bool> AllScopesExistAsync(List<Guid> scopeIds)
        {
            const string sql = @"
                SELECT COUNT(ScopeId)
                FROM Scopes
                WHERE ScopeId IN @ScopeIds
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                new { ScopeIds = scopeIds },
                transaction: Tx
            );

            return count == scopeIds.Count;
        }
    }
}
