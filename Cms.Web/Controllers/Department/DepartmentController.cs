using Cms.Contract.Services.Department.Interfaces;
using Cms.Web.Controllers.Contracts.Api;
using Cms.Web.Controllers.Department.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Department
{
    [ApiController]
    [Route("api/department")]
    [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
    }
}
