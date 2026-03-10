using System.Data;
using Dapper;
using Cms.Contract.Repositories.ContentType.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Contract.Repositories.ContentType.Entities;

namespace Cms.Infrastructure.Repositories.ContentType
{
    public class ContentTypeRepository : BaseRepository, IContentTypeRepository
    {
        public ContentTypeRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ): base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<ContentTypeOptionEntity>> GetContentTypeOptionsAsync(Guid departmentId)
        {
            const string sql = @"
                SELECT
                    TypeId,
                    TypeCode,
                    TypeName
                FROM ContentTypes
                WHERE DepartmentId = @DepartmentId
                AND IsEnabled = 1
                ORDER BY TypeName
            ";

            return await _db.QueryAsync<ContentTypeOptionEntity>(
                sql,
                new
                {
                    DepartmentId = departmentId
                },
                transaction: Tx
            );
        }

        public async Task<IEnumerable<ContentFieldEntity>> GetContentFieldsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    cf.FieldId,
                    cf.FieldCode,
                    cf.FieldType,
                    cf.IsRequired
                FROM ContentTypeFields ctf
                INNER JOIN ContentFields cf
                    ON cf.FieldId = ctf.FieldId
                WHERE ctf.TypeId = @TypeId
                ORDER BY ctf.SortOrder, cf.FieldCode
            ";

            return await _db.QueryAsync<ContentFieldEntity>(
                sql,
                new
                {
                    TypeId = typeId
                },
                transaction: Tx
            );
        }

        public async Task<bool> TypeNameExistsAsync(Guid departmentId, string typeName)
        {
            const string sql = @"
                SELECT 1
                FROM ContentTypes
                WHERE DepartmentId = @DepartmentId
                AND TypeName = @TypeName
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    DepartmentId = departmentId,
                    TypeName = typeName
                },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<Guid?> GetTypeIdByDepartmentIdAndTypeNameAsync(Guid departmentId, string typeName)
        {
            const string sql = @"
                SELECT TypeId
                FROM ContentTypes
                WHERE DepartmentId = @DepartmentId
                AND TypeName = @TypeName
            ";

            return await _db.ExecuteScalarAsync<Guid?>(
                sql,
                new
                {
                    DepartmentId = departmentId,
                    TypeName = typeName
                },
                transaction: Tx
            );
        }

        public async Task<bool> TypeExistsAsync(Guid typeId)
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

        public async Task<Guid> CreateContentTypeAsync(Guid departmentId, string typeName, string typeCode)
        {
            const string sql = @"
                INSERT INTO ContentTypes
                (
                    TypeId,
                    DepartmentId,
                    TypeCode,
                    TypeName,
                    IsEnabled,
                    CreatedAt
                )
                OUTPUT INSERTED.TypeId
                VALUES
                (
                    NEWID(),
                    @DepartmentId,
                    @TypeCode,
                    @TypeName,
                    1,
                    SYSUTCDATETIME()
                );
            ";

            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    DepartmentId = departmentId,
                    TypeName = typeName,
                    TypeCode = typeCode
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentTypeAsync(Guid typeId, string typeName)
        {
            const string sql = @"
                UPDATE ContentTypes
                SET TypeName = @TypeName
                WHERE TypeId = @TypeId
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = typeId,
                    TypeName = typeName
                },
                transaction: Tx
            );
        }

        public async Task<Guid> CreateContentFieldAsync(string fieldCode, string fieldType, bool isRequired)
        {
            const string sql = @"
                INSERT INTO ContentFields
                (
                    FieldId,
                    FieldCode,
                    FieldType,
                    IsRequired,
                    CreatedAt
                )
                OUTPUT INSERTED.FieldId
                VALUES
                (
                    NEWID(),
                    @FieldCode,
                    @FieldType,
                    @IsRequired,
                    SYSUTCDATETIME()
                );
            ";

            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    FieldCode = fieldCode,
                    FieldType = fieldType,
                    IsRequired = isRequired
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentFieldAsync(Guid fieldId, string fieldCode, string fieldType, bool isRequired)
        {
            const string sql = @"
                UPDATE ContentFields
                SET
                    FieldCode = @FieldCode,
                    FieldType = @FieldType,
                    IsRequired = @IsRequired
                WHERE FieldId = @FieldId
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    FieldId = fieldId,
                    FieldCode = fieldCode,
                    FieldType = fieldType,
                    IsRequired = isRequired
                },
                transaction: Tx
            );
        }

        public async Task CreateContentTypeFieldAsync(Guid typeId, Guid fieldId, int sortOrder)
        {
            const string sql = @"
                INSERT INTO ContentTypeFields
                (
                    TypeId,
                    FieldId,
                    SortOrder
                )
                VALUES
                (
                    @TypeId,
                    @FieldId,
                    @SortOrder
                );
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = typeId,
                    FieldId = fieldId,
                    SortOrder = sortOrder
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentTypeFieldSortOrderAsync(Guid typeId, Guid fieldId, int sortOrder)
        {
            const string sql = @"
                UPDATE ContentTypeFields
                SET SortOrder = @SortOrder
                WHERE TypeId = @TypeId
                AND FieldId = @FieldId
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = typeId,
                    FieldId = fieldId,
                    SortOrder = sortOrder
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentTypeFieldAsync(Guid typeId, Guid fieldId)
        {
            const string sql = @"
                DELETE FROM ContentTypeFields
                WHERE TypeId = @TypeId
                AND FieldId = @FieldId
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = typeId,
                    FieldId = fieldId
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentFieldAsync(Guid fieldId)
        {
            const string sql = @"
                DELETE FROM ContentFields
                WHERE FieldId = @FieldId
            ";

            await _db.ExecuteAsync(
                sql,
                new { FieldId = fieldId },
                transaction: Tx
            );
        }

        public async Task DeleteContentTypeAsync(Guid typeId)
        {
            const string sql = @"
                DELETE FROM ContentTypes
                WHERE TypeId = @TypeId
            ";

            await _db.ExecuteAsync(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );
        }

        public async Task<bool> ContentExistsByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT 1
                FROM Contents
                WHERE TypeId = @TypeId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { TypeId = typeId },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentFieldValueExistsAsync(Guid fieldId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentFieldValues
                WHERE FieldId = @FieldId
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { FieldId = fieldId },
                transaction: Tx
            );

            return result.HasValue;
        }
    }
}
