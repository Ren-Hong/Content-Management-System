using System.Security.Claims;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Content.Models;
using Cms.Contract.Services.Content.Dtos;
using Cms.Contract.Services.Content.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Content
{
    [ApiController]
    [Route("api/content")]
    [Authorize]
    public class ContentController : Controller
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("{typeId:guid}")]
        public async Task<IActionResult> GetContents(Guid typeId)
        {
            var serviceResponse = await _contentService.GetContentsAsync(typeId, GetCurrentAccountId());

            return Json(new ApiResponse<IEnumerable<GetContentsResponseModel>>
            {
                Success = true,
                Data = serviceResponse.Select(content => new GetContentsResponseModel
                {
                    ContentId = content.ContentId,
                    RevisionId = content.RevisionId,
                    Version = content.Version,
                    OwnerUsername = content.OwnerUsername,
                    Status = content.Status,
                    CreatedAt = content.CreatedAt,
                    FieldValues = content.FieldValues
                        .Select(field => new GetContentFieldValueModel
                        {
                            FieldId = field.FieldId,
                            FieldName = field.FieldName,
                            FieldType = field.FieldType,
                            FieldValue = field.FieldValue
                        })
                        .ToList()
                }).ToList()
            });
        }

        [Authorize(Policy = "Permission.Content.Create")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateContent([FromBody] CreateContentRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(BuildValidationErrorResponse());
            }

            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = Guid.TryParse(ownerClaim, out var parsedOwnerId)
                ? parsedOwnerId
                : Guid.Empty;

            var serviceResponse = await _contentService.CreateContentAsync(
                new CreateContentRequestDto
                {
                    TypeId = requestModel.TypeId,
                    OwnerId = ownerId,
                    FieldValues = requestModel.FieldValues
                        .Select(fieldValue => new CreateContentFieldValueDto
                        {
                            FieldId = fieldValue.FieldId,
                            FieldValue = fieldValue.FieldValue
                        })
                        .ToList()
                }
            );

            return Json(new ApiResponse<CreateContentResponseModel>
            {
                Success = serviceResponse.Result == CreateContentResult.Success,
                ErrorCode = serviceResponse.Result != CreateContentResult.Success
                    ? serviceResponse.Result.ToString()
                    : null,
                Data = serviceResponse.Result == CreateContentResult.Success
                    ? new CreateContentResponseModel
                    {
                        ContentId = serviceResponse.ContentId!.Value,
                        RevisionId = serviceResponse.RevisionId!.Value
                    }
                    : null
            });
        }

        [Authorize(Policy = "Permission.Content.Edit")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateContent([FromBody] UpdateContentRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(BuildValidationErrorResponse());
            }

            var serviceResponse = await _contentService.UpdateContentAsync(
                new UpdateContentRequestDto
                {
                    ContentId = requestModel.ContentId,
                    RevisionId = requestModel.RevisionId,
                    FieldValues = requestModel.FieldValues
                        .Select(fieldValue => new UpdateContentFieldValueDto
                        {
                            FieldId = fieldValue.FieldId,
                            FieldValue = fieldValue.FieldValue
                        })
                        .ToList()
                },
                GetCurrentAccountId()
            );

            return Json(new ApiResponse
            {
                Success = serviceResponse.Result == UpdateContentResult.Success,
                ErrorCode = serviceResponse.Result != UpdateContentResult.Success
                    ? serviceResponse.Result.ToString()
                    : null
            });
        }

        [Authorize(Policy = "Permission.Content.Delete")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteContent([FromBody] DeleteContentRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(BuildValidationErrorResponse());
            }

            var serviceResponse = await _contentService.DeleteContentAsync(
                new DeleteContentRequestDto
                {
                    ContentId = requestModel.ContentId
                },
                GetCurrentAccountId()
            );

            return Json(new ApiResponse
            {
                Success = serviceResponse.Result == DeleteContentResult.Success,
                ErrorCode = serviceResponse.Result != DeleteContentResult.Success
                    ? serviceResponse.Result.ToString()
                    : null
            });
        }

        private ApiResponse BuildValidationErrorResponse()
        {
            return new ApiResponse
            {
                Success = false,
                ValidationErrors = ModelState
                    .Where(entry => entry.Value!.Errors.Any())
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToList()
                    )
            };
        }

        private Guid GetCurrentAccountId()
        {
            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(ownerClaim, out var accountId)
                ? accountId
                : Guid.Empty;
        }
    }
}
