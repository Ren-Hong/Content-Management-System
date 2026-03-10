using Cms.Contract.Repositories.Content.Interfaces;
using Cms.Contract.Services.Content.Dtos;
using Cms.Contract.Services.Content.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Content
{
    public class ContentService : IContentService
    {
        private const string DefaultContentStatus = "Draft";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IContentRepository _contentRepository;

        public ContentService(
            IUnitOfWork unitOfWork,
            IContentRepository contentRepository)
        {
            _unitOfWork = unitOfWork;
            _contentRepository = contentRepository;
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
    }
}
