using Cms.Application.Services.Role;
using Cms.Application.Services.Role.Dtos;
using Cms.Web.Controllers.Contracts.Api;
using Cms.Web.Controllers.Permission.Models;
using Cms.Web.Controllers.Role.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Role
{
    [ApiController]
    [Route("api/role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(
            IRoleService roleService
        )
        {
            _roleService = roleService;
        }

        [HttpPost("options")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoleOptions()
        {
            var rdto = await _roleService.GetRoleOptionsAsync();

            var res = rdto.Select(x => new GetRoleOptionsResponseModel
            {
                RoleName = x.RoleName,
                RoleId = x.RoleId,
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetRoleOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("summaries")]
        public async Task<IActionResult> GetRoleSummaries()
        {
            var rdto = await _roleService.GetRoleSummariesAsync();

            var res = rdto.Select(x => new GetRoleSummariesResponseModel // 跟 dto 長依樣
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                Status = x.Status,

                Permissions = x.Permissions.Select(r => new GetPermissionOptionsResponseModel
                {
                    PermissionId = r.PermissionId,
                    PermissionName = r.PermissionName
                }).ToList()

            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetRoleSummariesResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestModel req)
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

            var rdto = await _roleService.CreateRoleAsync(
                new CreateRoleRequestDto
                {
                    RoleName = req.RoleName,
                    RoleCode = req.RoleCode,
                    PermissionIds = req.PermissionIds
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == CreateRoleResult.Success,
                ErrorCode = rdto.Result != CreateRoleResult.Success
                    ? rdto.Result.ToString() // 這邊要ToString喔, 不然前端會拿到魔法數字
                    : null
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequestModel req)
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

            var rdto = await _roleService.UpdateRoleAsync(
                new UpdateRoleRequestDto
                {
                    RoleName = req.RoleName,
                    PermissionIds = req.PermissionIds,
                    Status = req.Status,
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == UpdateRoleResult.Success,
                ErrorCode = rdto.Result != UpdateRoleResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleRequestModel req)
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

            var rdto = await _roleService.DeleteRoleAsync(
                new DeleteRoleRequestDto
                {
                    RoleName = req.RoleName,
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == DeleteRoleResult.Success,
                ErrorCode = rdto.Result != DeleteRoleResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }

    }
}
