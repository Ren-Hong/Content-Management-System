using System.Data;
using Cms.Contract.Repositories.Content.Entities;
using Cms.Contract.Repositories.Content.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;

namespace Cms.Infrastructure.Repositories.Content
{
    public class ContentRepository : BaseRepository, IContentRepository
    {
        public ContentRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<bool> ContentTypeExistsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentTypes
                WHERE TypeId = @TypeId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentExistsAsync(Guid contentId)
        {
            const string sql = @"
                SELECT 1
                FROM Contents
                WHERE ContentId = @ContentId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { ContentId = contentId },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentRevisionExistsAsync(Guid revisionId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentRevisions
                WHERE RevisionId = @RevisionId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { RevisionId = revisionId },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentRevisionBelongsToContentAsync(Guid contentId, Guid revisionId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentRevisions
                WHERE ContentId = @ContentId
                    AND RevisionId = @RevisionId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    ContentId = contentId,
                    RevisionId = revisionId
                },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<Guid?> GetTypeIdByRevisionIdAsync(Guid revisionId)
        {
            const string sql = @"
                SELECT c.TypeId
                FROM ContentRevisions cr
                INNER JOIN Contents c
                    ON c.ContentId = cr.ContentId
                WHERE cr.RevisionId = @RevisionId
            ";

            return await _db.ExecuteScalarAsync<Guid?>(
                sql,
                new { RevisionId = revisionId },
                transaction: Tx
            );
        }

        public async Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT DepartmentId
                FROM ContentTypes
                WHERE TypeId = @TypeId
            ";

            return await _db.ExecuteScalarAsync<Guid?>(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );
        }

        public async Task<Guid?> GetDepartmentIdByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                SELECT ct.DepartmentId
                FROM Contents c
                INNER JOIN ContentTypes ct
                    ON ct.TypeId = c.TypeId
                WHERE c.ContentId = @ContentId
            ";

            return await _db.ExecuteScalarAsync<Guid?>(
                sql,
                new { ContentId = contentId },
                transaction: Tx
            );
        }

        public async Task<List<ContentTypeFieldDefinitionEntity>> GetFieldDefinitionsByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    cf.FieldId,
                    cf.FieldType,
                    cf.IsRequired
                FROM ContentTypeFields ctf
                INNER JOIN ContentFields cf
                    ON cf.FieldId = ctf.FieldId
                WHERE ctf.TypeId = @TypeId
                ORDER BY ctf.SortOrder, cf.FieldCode
            ";

            var fields = await _db.QueryAsync<ContentTypeFieldDefinitionEntity>(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );

            return fields.ToList();
        }

        public async Task<List<ContentEntity>> GetContentsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    c.ContentId,
                    cr.RevisionId,
                    cr.Version,
                    c.OwnerId,
                    a.Username AS OwnerUsername,
                    c.Status,
                    cr.CreatedAt,
                    cf.FieldId,
                    cf.FieldCode,
                    cf.FieldType,
                    cfv.FieldValue,
                    ISNULL(ctf.SortOrder, 0) AS SortOrder
                FROM Contents c
                INNER JOIN ContentRevisions cr
                    ON cr.ContentId = c.ContentId
                INNER JOIN Accounts a
                    ON a.AccountId = c.OwnerId
                INNER JOIN ContentFieldValues cfv
                    ON cfv.RevisionId = cr.RevisionId
                INNER JOIN ContentFields cf
                    ON cf.FieldId = cfv.FieldId
                LEFT JOIN ContentTypeFields ctf
                    ON ctf.TypeId = c.TypeId
                    AND ctf.FieldId = cf.FieldId
                WHERE c.TypeId = @TypeId
                ORDER BY cr.CreatedAt DESC, ISNULL(ctf.SortOrder, 0), cf.FieldCode
            ";

            var rows = await _db.QueryAsync<ContentEntity>(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );

            return rows.ToList();
        }

        public async Task<Guid> CreateContentAsync(Guid typeId, Guid ownerId, string status)
        {
            const string sql = @"
                INSERT INTO Contents
                (
                    ContentId,
                    TypeId,
                    OwnerId,
                    Status,
                    CreatedAt
                )
                OUTPUT INSERTED.ContentId
                VALUES
                (
                    NEWID(),
                    @TypeId,
                    @OwnerId,
                    @Status,
                    SYSUTCDATETIME()
                );
            ";

            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    TypeId = typeId,
                    OwnerId = ownerId,
                    Status = status
                },
                transaction: Tx
            );
        }

        public async Task<Guid> CreateContentRevisionAsync(Guid contentId, int version)
        {
            const string sql = @"
                INSERT INTO ContentRevisions
                (
                    RevisionId,
                    ContentId,
                    Version,
                    CreatedAt
                )
                OUTPUT INSERTED.RevisionId
                VALUES
                (
                    NEWID(),
                    @ContentId,
                    @Version,
                    SYSUTCDATETIME()
                );
            ";

            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    ContentId = contentId,
                    Version = version
                },
                transaction: Tx
            );
        }

        public async Task CreateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue)
        {
            const string sql = @"
                INSERT INTO ContentFieldValues
                (
                    RevisionId,
                    FieldId,
                    FieldValue
                )
                VALUES
                (
                    @RevisionId,
                    @FieldId,
                    @FieldValue
                );
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RevisionId = revisionId,
                    FieldId = fieldId,
                    FieldValue = fieldValue
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue)
        {
            const string sql = @"
                UPDATE ContentFieldValues
                SET FieldValue = @FieldValue
                WHERE RevisionId = @RevisionId
                    AND FieldId = @FieldId
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RevisionId = revisionId,
                    FieldId = fieldId,
                    FieldValue = fieldValue
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentFieldValuesByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                DELETE cfv
                FROM ContentFieldValues cfv
                INNER JOIN ContentRevisions cr
                    ON cr.RevisionId = cfv.RevisionId
                WHERE cr.ContentId = @ContentId
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = contentId },
                transaction: Tx
            );
        }

        public async Task DeleteContentRevisionsByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                DELETE FROM ContentRevisions
                WHERE ContentId = @ContentId
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = contentId },
                transaction: Tx
            );
        }

        public async Task DeleteContentAsync(Guid contentId)
        {
            const string sql = @"
                DELETE FROM Contents
                WHERE ContentId = @ContentId
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = contentId },
                transaction: Tx
            );
        }
    }
}
