using Cms.Application.Services.Permission;
using Cms.Web.Controllers.Contracts.Api;
using Cms.Web.Controllers.Permission.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Permission
{
    [ApiController]
    [Route("api/permission")]
    [Authorize(Roles = "Admin")]
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(
            IPermissionService permissionService
        )
        {
            _permissionService = permissionService;
        }

        [HttpPost("options")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPermissionOptions()
        {
            var rdto = await _permissionService.GetPermissionOptionsAsync();

            var res = rdto.Select(x => new GetPermissionOptionsResponseModel
            {
                PermissionName = x.PermissionName,
                PermissionId = x.PermissionId,
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetPermissionOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }
    }
}
