using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Services.ContentType.Interfaces;
using Cms.Contract.Controllers.ContentType.Models;
using Cms.Contract.Services.ContentType.Dtos;

namespace Cms.Web.Controllers.ContentType
{
    [ApiController]
    [Route("api/contenttype")]
    [Authorize]
    public class ContentTypeController : Controller
    {
        private readonly IContentTypeService _contentTypeService;

        public ContentTypeController(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService;
        }

        [HttpPost("options")]
        public async Task<IActionResult> GetContentTypeOptions([FromBody] GetContentTypeOptionsRequestModel req)
        {
            var dto = new GetContentTypeOptionsRequestDto
            {
                DepartmentId = req.DepartmentId
            };

            var rdto = await _contentTypeService.GetContentTypeOptionsAsync(dto);

            var res = rdto.Select(x => new GetContentTypeOptionsResponseModel
            {
                TypeId = x.TypeId,
                TypeName = x.TypeName
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetContentTypeOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContentType([FromBody] CreateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ValidationErrors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                        )
                });
            }

            var rdto = await _contentTypeService.CreateContentTypeAsync(
                new CreateContentTypeRequestDto
                {
                    DepartmentId = req.DepartmentId,
                    TypeName = req.TypeName,
                    Fields = req.Fields
                        .Select(x => new CreateContentTypeFieldDto
                        {
                            FieldName = x.FieldName,
                            FieldType = x.FieldType,
                            IsRequired = x.IsRequired
                        })
                        .ToList()
                }
            );

            return Json(new ApiResponse<CreateContentTypeResponseModel>
            {
                Success = rdto.Result == CreateContentTypeResult.Success,
                ErrorCode = rdto.Result != CreateContentTypeResult.Success
                    ? rdto.Result.ToString()
                    : null,
                Data = rdto.Result == CreateContentTypeResult.Success
                    ? new CreateContentTypeResponseModel
                    {
                        TypeId = rdto.TypeId!.Value,
                        TypeName = rdto.TypeName!
                    }
                    : null
            });
        }

        [HttpGet("{typeId:guid}/fields")]
        public async Task<IActionResult> GetContentFields(Guid typeId)
        {
            var rdto = await _contentTypeService.GetContentFieldsAsync(typeId);

            var res = rdto.Select(x => new GetContentFieldsResponseModel
            {
                FieldId = x.FieldId,
                FieldName = x.FieldName,
                FieldCode = x.FieldCode,
                FieldType = x.FieldType,
                IsRequired = x.IsRequired
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetContentFieldsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateContentType([FromBody] UpdateContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ValidationErrors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                        )
                });
            }

            var rdto = await _contentTypeService.UpdateContentTypeAsync(
                new UpdateContentTypeRequestDto
                {
                    TypeId = req.TypeId,
                    TypeName = req.TypeName,
                    Fields = req.Fields
                        .Select(x => new UpdateContentTypeFieldDto
                        {
                            FieldId = x.FieldId,
                            FieldName = x.FieldName,
                            FieldType = x.FieldType,
                            IsRequired = x.IsRequired
                        })
                        .ToList()
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == UpdateContentTypeResult.Success,
                ErrorCode = rdto.Result != UpdateContentTypeResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteContentType([FromBody] DeleteContentTypeRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ValidationErrors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                        )
                });
            }

            var rdto = await _contentTypeService.DeleteContentTypeAsync(
                new DeleteContentTypeRequestDto
                {
                    TypeId = req.TypeId
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == DeleteContentTypeResult.Success,
                ErrorCode = rdto.Result != DeleteContentTypeResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }
    }
}
