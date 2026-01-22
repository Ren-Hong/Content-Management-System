using Cms.Web.Controllers.Contracts.Api;
using Cms.Web.Controllers.Role.Models;
using Cms.Application.Services.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Role
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
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
    }
}
