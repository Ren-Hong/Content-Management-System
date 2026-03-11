using Cms.Contract.Repositories.Content.Interfaces;
using Cms.Contract.Services.Content.Dtos;
using Cms.Contract.Services.Content.Interfaces;
using Cms.Contract.Services.PermissionScope.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Content
{
    public class ContentService : IContentService
    {
        private const string DefaultContentStatus = "Draft";
        private const string ContentViewPermissionCode = "Content.View";
        private const string ContentCreatePermissionCode = "Content.Create";
        private const string ContentEditPermissionCode = "Content.Edit";
        private const string ContentDeletePermissionCode = "Content.Delete";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IContentRepository _contentRepository;
        private readonly IPermissionScopeService _permissionScopeService;

        public ContentService(
            IUnitOfWork unitOfWork,
            IContentRepository contentRepository,
            IPermissionScopeService permissionScopeService)
        {
            _unitOfWork = unitOfWork;
            _contentRepository = contentRepository;
            _permissionScopeService = permissionScopeService;
        }

        public async Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto)
        {
            if (requestDto.TypeId == Guid.Empty || !await _contentRepository.ContentTypeExistsAsync(requestDto.TypeId))
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.TypeNotFound
                };
            }

            if (requestDto.OwnerId == Guid.Empty)
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.OwnerRequired
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByTypeIdAsync(requestDto.TypeId);
            var canCreateInDepartment = departmentId.HasValue && await _permissionScopeService.CanAccessDepartmentAsync(
                requestDto.OwnerId,
                ContentCreatePermissionCode,
                departmentId.Value
            );

            if (!canCreateInDepartment)
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.PermissionDenied
                };
            }

            if (requestDto.FieldValues == null || !requestDto.FieldValues.Any())
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.FieldValueRequired
                };
            }

            var fieldDefinitions = await _contentRepository.GetFieldDefinitionsByTypeIdAsync(requestDto.TypeId);
            var fieldDefinitionMap = fieldDefinitions.ToDictionary(field => field.FieldId);

            var providedFieldIds = requestDto.FieldValues
                .Select(fieldValue => fieldValue.FieldId)
                .ToHashSet();

            if (providedFieldIds.Any(fieldId => !fieldDefinitionMap.ContainsKey(fieldId)))
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.FieldNotFound
                };
            }

            foreach (var fieldDefinition in fieldDefinitions.Where(field => field.IsRequired))
            {
                var providedFieldValue = requestDto.FieldValues
                    .FirstOrDefault(fieldValue => fieldValue.FieldId == fieldDefinition.FieldId);

                if (providedFieldValue == null || string.IsNullOrWhiteSpace(providedFieldValue.FieldValue))
                {
                    return new CreateContentResponseDto
                    {
                        Result = CreateContentResult.RequiredFieldMissing
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var contentId = await _contentRepository.CreateContentAsync(
                    requestDto.TypeId,
                    requestDto.OwnerId,
                    DefaultContentStatus
                );

                var revisionId = await _contentRepository.CreateContentRevisionAsync(contentId, 1);

                foreach (var fieldValue in requestDto.FieldValues)
                {
                    await _contentRepository.CreateContentFieldValueAsync(
                        revisionId,
                        fieldValue.FieldId,
                        fieldValue.FieldValue?.Trim()
                    );
                }

                _unitOfWork.Commit();

                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.Success,
                    ContentId = contentId,
                    RevisionId = revisionId
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<List<GetContentsResponseDto>> GetContentsAsync(Guid typeId, Guid accountId)
        {
            if (typeId == Guid.Empty || !await _contentRepository.ContentTypeExistsAsync(typeId))
            {
                return [];
            }

            var departmentId = await _contentRepository.GetDepartmentIdByTypeIdAsync(typeId);
            var canViewDepartment = departmentId.HasValue && await _permissionScopeService.CanAccessDepartmentAsync(
                accountId,
                ContentViewPermissionCode,
                departmentId.Value
            );

            if (!canViewDepartment)
            {
                return [];
            }

            var rows = await _contentRepository.GetContentsAsync(typeId);

            return rows
                .GroupBy(
                    row => new
                    {
                        row.ContentId,
                        row.RevisionId,
                        row.Version,
                        row.OwnerUsername,
                        row.Status,
                        row.CreatedAt
                    }
                )
                .Select(group => new GetContentsResponseDto
                {
                    ContentId = group.Key.ContentId,
                    RevisionId = group.Key.RevisionId,
                    Version = group.Key.Version,
                    OwnerUsername = group.Key.OwnerUsername,
                    Status = group.Key.Status,
                    CreatedAt = group.Key.CreatedAt,
                    FieldValues = group
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.FieldCode)
                        .Select(x => new GetContentFieldValueDto
                        {
                            FieldId = x.FieldId,
                            FieldName = GetFieldDisplayName(x.FieldCode),
                            FieldType = x.FieldType,
                            FieldValue = x.FieldValue
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task<UpdateContentResponseDto> UpdateContentAsync(UpdateContentRequestDto requestDto, Guid accountId)
        {
            if (requestDto.ContentId == Guid.Empty || !await _contentRepository.ContentExistsAsync(requestDto.ContentId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.ContentNotFound
                };
            }

            if (requestDto.RevisionId == Guid.Empty || !await _contentRepository.ContentRevisionExistsAsync(requestDto.RevisionId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            if (!await _contentRepository.ContentRevisionBelongsToContentAsync(requestDto.ContentId, requestDto.RevisionId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByContentIdAsync(requestDto.ContentId);
            var canEditInDepartment = departmentId.HasValue && await _permissionScopeService.CanAccessDepartmentAsync(
                accountId,
                ContentEditPermissionCode,
                departmentId.Value
            );

            if (!canEditInDepartment)
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.PermissionDenied
                };
            }

            if (requestDto.FieldValues == null || !requestDto.FieldValues.Any())
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.FieldValueRequired
                };
            }

            var typeId = await _contentRepository.GetTypeIdByRevisionIdAsync(requestDto.RevisionId);
            if (!typeId.HasValue)
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            var fieldDefinitions = await _contentRepository.GetFieldDefinitionsByTypeIdAsync(typeId.Value);
            var fieldDefinitionMap = fieldDefinitions.ToDictionary(field => field.FieldId);

            var providedFieldIds = requestDto.FieldValues
                .Select(fieldValue => fieldValue.FieldId)
                .ToHashSet();

            if (providedFieldIds.Any(fieldId => !fieldDefinitionMap.ContainsKey(fieldId)))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.FieldNotFound
                };
            }

            foreach (var fieldDefinition in fieldDefinitions.Where(field => field.IsRequired))
            {
                var providedFieldValue = requestDto.FieldValues
                    .FirstOrDefault(fieldValue => fieldValue.FieldId == fieldDefinition.FieldId);

                if (providedFieldValue == null || string.IsNullOrWhiteSpace(providedFieldValue.FieldValue))
                {
                    return new UpdateContentResponseDto
                    {
                        Result = UpdateContentResult.RequiredFieldMissing
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                foreach (var fieldValue in requestDto.FieldValues)
                {
                    await _contentRepository.UpdateContentFieldValueAsync(
                        requestDto.RevisionId,
                        fieldValue.FieldId,
                        fieldValue.FieldValue?.Trim()
                    );
                }

                _unitOfWork.Commit();

                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteContentResponseDto> DeleteContentAsync(DeleteContentRequestDto requestDto, Guid accountId)
        {
            if (requestDto.ContentId == Guid.Empty || !await _contentRepository.ContentExistsAsync(requestDto.ContentId))
            {
                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.ContentNotFound
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByContentIdAsync(requestDto.ContentId);
            var canDeleteInDepartment = departmentId.HasValue && await _permissionScopeService.CanAccessDepartmentAsync(
                accountId,
                ContentDeletePermissionCode,
                departmentId.Value
            );

            if (!canDeleteInDepartment)
            {
                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.PermissionDenied
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _contentRepository.DeleteContentFieldValuesByContentIdAsync(requestDto.ContentId);
                await _contentRepository.DeleteContentRevisionsByContentIdAsync(requestDto.ContentId);
                await _contentRepository.DeleteContentAsync(requestDto.ContentId);

                _unitOfWork.Commit();

                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private static string GetFieldDisplayName(string fieldCode)
        {
            const string separator = "__";
            var separatorIndex = fieldCode.LastIndexOf(separator, StringComparison.Ordinal);

            return separatorIndex > 0
                ? fieldCode[..separatorIndex]
                : fieldCode;
        }
    }
}
