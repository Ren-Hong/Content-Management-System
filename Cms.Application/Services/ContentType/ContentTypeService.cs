using Cms.Contract.Services.ContentType.Interfaces;
using Cms.Contract.Repositories.ContentType.Interfaces;
using Cms.Contract.Services.ContentType.Dtos;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.ContentType
{
    public class ContentTypeService : IContentTypeService
    {
        private static readonly HashSet<string> AllowedFieldTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "text",
            "number",
            "date",
            "textarea"
        };

        private readonly IUnitOfWork _unitOfWork;
        private readonly IContentTypeRepository _contentTypeRepository;
        public ContentTypeService
        (
            IUnitOfWork unitOfWork,
            IContentTypeRepository contentTypeRepository
        )
        {
            _unitOfWork = unitOfWork;
            _contentTypeRepository = contentTypeRepository;
        }

        
        public async Task<List<GetContentTypeOptionsResponseDto>> GetContentTypeOptionsAsync(GetContentTypeOptionsRequestDto req)
        {
            var rows = await _contentTypeRepository.GetContentTypeOptionsAsync(req.DepartmentId);

            return rows
                .Select(x => new GetContentTypeOptionsResponseDto
                {
                    TypeId = x.TypeId,
                    TypeName = x.TypeName
                })
                .ToList();
        }

        public async Task<List<GetContentFieldsResponseDto>> GetContentFieldsAsync(Guid typeId)
        {
            var rows = await _contentTypeRepository.GetContentFieldsAsync(typeId);

            return rows
                .Select(x => new GetContentFieldsResponseDto
                {
                    FieldId = x.FieldId,
                    FieldName = GetFieldDisplayName(x.FieldCode),
                    FieldCode = x.FieldCode,
                    FieldType = x.FieldType,
                    IsRequired = x.IsRequired
                })
                .ToList();
        }

        public async Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req)
        {
            if (req.DepartmentId == Guid.Empty)
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.DepartmentRequired
                };
            }

            var typeName = req.TypeName?.Trim();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.TypeNameRequired
                };
            }

            if (await _contentTypeRepository.TypeNameExistsAsync(req.DepartmentId, typeName))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.TypeNameDuplicated
                };
            }

            if (req.Fields == null || !req.Fields.Any())
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldRequired
                };
            }

            var normalizedFields = req.Fields
                .Select(x => new CreateContentTypeFieldDto
                {
                    FieldName = x.FieldName?.Trim() ?? string.Empty,
                    FieldType = x.FieldType?.Trim().ToLowerInvariant() ?? string.Empty,
                    IsRequired = x.IsRequired
                })
                .ToList();

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldNameRequired
                };
            }

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldType)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldTypeRequired
                };
            }

            var duplicatedFieldNames = normalizedFields
                .GroupBy(x => x.FieldName, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1);

            if (duplicatedFieldNames)
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldNameDuplicated
                };
            }

            if (normalizedFields.Any(x => !AllowedFieldTypes.Contains(x.FieldType)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldTypeInvalid
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var typeId = await _contentTypeRepository.CreateContentTypeAsync(
                    req.DepartmentId,
                    typeName,
                    GenerateTypeCode()
                );

                for (var i = 0; i < normalizedFields.Count; i++)
                {
                    var field = normalizedFields[i];
                    var fieldId = await _contentTypeRepository.CreateContentFieldAsync(
                        GenerateFieldCode(field.FieldName),
                        field.FieldType,
                        field.IsRequired
                    );

                    await _contentTypeRepository.CreateContentTypeFieldAsync(
                        typeId,
                        fieldId,
                        i
                    );
                }

                _unitOfWork.Commit();

                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.Success,
                    TypeId = typeId,
                    TypeName = typeName
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private static string GenerateTypeCode()
        {
            return $"CT-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N")[..8]}";
        }

        private static string GenerateFieldCode(string fieldName)
        {
            return $"{fieldName}__{Guid.NewGuid().ToString("N")[..8]}";
        }

        private static string GetFieldDisplayName(string fieldCode)
        {
            const string separator = "__";
            var separatorIndex = fieldCode.LastIndexOf(separator, StringComparison.Ordinal);

            return separatorIndex > 0
                ? fieldCode[..separatorIndex]
                : fieldCode;
        }

        public async Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req)
        {
            if (req.TypeId == Guid.Empty || !await _contentTypeRepository.TypeExistsAsync(req.TypeId))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNotFound
                };
            }

            var typeName = req.TypeName?.Trim();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNameRequired
                };
            }

            var departmentId = await _contentTypeRepository.GetDepartmentIdByTypeIdAsync(req.TypeId);
            if (!departmentId.HasValue)
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNotFound
                };
            }

            if (req.Fields == null || !req.Fields.Any())
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldRequired
                };
            }

            var normalizedFields = req.Fields
                .Select(x => new UpdateContentTypeFieldDto
                {
                    FieldId = x.FieldId,
                    FieldName = x.FieldName?.Trim() ?? string.Empty,
                    FieldType = x.FieldType?.Trim().ToLowerInvariant() ?? string.Empty,
                    IsRequired = x.IsRequired
                })
                .ToList();

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNameRequired
                };
            }

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldType)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldTypeRequired
                };
            }

            if (normalizedFields
                .GroupBy(x => x.FieldName, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNameDuplicated
                };
            }

            if (normalizedFields.Any(x => !AllowedFieldTypes.Contains(x.FieldType)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldTypeInvalid
                };
            }

            var currentFields = (await _contentTypeRepository.GetContentFieldsAsync(req.TypeId)).ToList();
            var currentFieldIds = currentFields.Select(x => x.FieldId).ToHashSet();
            var requestedExistingFieldIds = normalizedFields
                .Where(x => x.FieldId.HasValue)
                .Select(x => x.FieldId!.Value)
                .ToList();

            if (requestedExistingFieldIds.Any(x => !currentFieldIds.Contains(x)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNotFound
                };
            }

            var removedFieldIds = currentFields
                .Where(x => !requestedExistingFieldIds.Contains(x.FieldId))
                .Select(x => x.FieldId)
                .ToList();

            foreach (var fieldId in removedFieldIds)
            {
                if (await _contentTypeRepository.ContentFieldValueExistsAsync(fieldId))
                {
                    return new UpdateContentTypeResponseDto
                    {
                        Result = UpdateContentTypeResult.FieldInUse
                    };
                }
            }

            foreach (var field in normalizedFields.Where(x => x.FieldId.HasValue))
            {
                var currentField = currentFields.First(x => x.FieldId == field.FieldId!.Value);

                if (!string.Equals(currentField.FieldType, field.FieldType, StringComparison.OrdinalIgnoreCase)
                    && await _contentTypeRepository.ContentFieldValueExistsAsync(currentField.FieldId))
                {
                    return new UpdateContentTypeResponseDto
                    {
                        Result = UpdateContentTypeResult.FieldInUse
                    };
                }
            }

            var duplicatedTypeId = await _contentTypeRepository.GetTypeIdByDepartmentIdAndTypeNameAsync(
                departmentId.Value,
                typeName
            );

            if (duplicatedTypeId.HasValue && duplicatedTypeId.Value != req.TypeId)
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNameDuplicated
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _contentTypeRepository.UpdateContentTypeAsync(req.TypeId, typeName);

                for (var i = 0; i < normalizedFields.Count; i++)
                {
                    var field = normalizedFields[i];

                    if (field.FieldId.HasValue)
                    {
                        await _contentTypeRepository.UpdateContentFieldAsync(
                            field.FieldId.Value,
                            GenerateFieldCode(field.FieldName),
                            field.FieldType,
                            field.IsRequired
                        );

                        await _contentTypeRepository.UpdateContentTypeFieldSortOrderAsync(
                            req.TypeId,
                            field.FieldId.Value,
                            i
                        );

                        continue;
                    }

                    var newFieldId = await _contentTypeRepository.CreateContentFieldAsync(
                        GenerateFieldCode(field.FieldName),
                        field.FieldType,
                        field.IsRequired
                    );

                    await _contentTypeRepository.CreateContentTypeFieldAsync(
                        req.TypeId,
                        newFieldId,
                        i
                    );
                }

                foreach (var fieldId in removedFieldIds)
                {
                    await _contentTypeRepository.DeleteContentTypeFieldAsync(req.TypeId, fieldId);
                    await _contentTypeRepository.DeleteContentFieldAsync(fieldId);
                }

                _unitOfWork.Commit();

                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req)
        {
            if (req.TypeId == Guid.Empty || !await _contentTypeRepository.TypeExistsAsync(req.TypeId))
            {
                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.TypeNotFound
                };
            }

            if (await _contentTypeRepository.ContentExistsByTypeIdAsync(req.TypeId))
            {
                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.ContentTypeInUse
                };
            }

            var fields = (await _contentTypeRepository.GetContentFieldsAsync(req.TypeId)).ToList();

            foreach (var field in fields)
            {
                if (await _contentTypeRepository.ContentFieldValueExistsAsync(field.FieldId))
                {
                    return new DeleteContentTypeResponseDto
                    {
                        Result = DeleteContentTypeResult.FieldInUse
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                foreach (var field in fields)
                {
                    await _contentTypeRepository.DeleteContentTypeFieldAsync(req.TypeId, field.FieldId);
                    await _contentTypeRepository.DeleteContentFieldAsync(field.FieldId);
                }

                await _contentTypeRepository.DeleteContentTypeAsync(req.TypeId);

                _unitOfWork.Commit();

                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
