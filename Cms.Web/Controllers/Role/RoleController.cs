using Cms.Contract.Services.Role.Dtos;
using Cms.Contract.Services.Role.Interfaces;
using Cms.Web.Controllers.Contracts.Api;
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
            var dtos = await _roleService.GetRoleSummariesAsync();

            var res = dtos
                .Select(x => new GetRoleSummariesResponseModel
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Status = x.Status,

                    PermissionScopes = x.PermissionScopes
                        .Select(p => new PermissionScopesSummaryModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,

                            Scopes = p.Scopes
                                .Select(s => new ScopeSummaryModel
                                {
                                    ScopeId = s.ScopeId,
                                    ScopeName = s.ScopeName
                                })
                                .ToList()

                        })
                        .ToList()
                })
                .ToList();

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

                    PermissionScopes = req.PermissionScopes
                        .Select(x => new PermissionScopeDto
                        {
                            PermissionId = x.PermissionId,
                            ScopeId = x.ScopeId
                        })
                        .ToList()
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
