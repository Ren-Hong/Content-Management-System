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
    }
}
