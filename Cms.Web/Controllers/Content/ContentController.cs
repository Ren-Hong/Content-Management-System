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
    }
}
