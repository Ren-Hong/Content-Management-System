using Cms.Contract.Controllers.Api;
using Cms.Contract.Services.Department.Interfaces;
using Cms.Contract.Controllers.Department.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers.Department
{
    [ApiController]
    [Route("api/department")]
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService
        )
        {
            _departmentService = departmentService;
        }

        [HttpPost("options")]
        public async Task<IActionResult> GetDepartmentOptions()
        {
            var rdto = await _departmentService.GetDepartmentOptionsAsync();

            var res = rdto.Select(x => new GetDepartmentOptionsResponseModel
            {
                DepartmentName = x.DepartmentName,
                DepartmentId = x.DepartmentId,
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetDepartmentOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost("sidebar")]
        [Authorize(Policy = "Permission.Content.View")]
        public async Task<IActionResult> GetDepartmentsForSidebar()
        {
            var rdto = await _departmentService.GetDepartmentsForSidebarAsync(GetCurrentAccountId());

            var res = rdto.Select(x => new GetDepartmentsForSidebarResponseModel
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                DepartmentCode = x.DepartmentCode,
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetDepartmentsForSidebarResponseModel>>
            {
                Success = true,
                Data = res
            });
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
