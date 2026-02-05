using Cms.Contract.Services.PermissionAssignment.Dtos;
using Cms.Contract.Services.PermissionAssignment.Interfaces;
using Cms.Web.Controllers.Contracts.Api;
using Cms.Web.Controllers.PermissionAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.PermissionAssignment
{
    [ApiController]
    [Route("api/permissionAssignment")]
    [Authorize(Roles = "Admin")]
    public class PermissionAssignmentController : Controller
    {
        private readonly IPermissionAssignmentService _permissionAssignmentService;

        public PermissionAssignmentController(
            IPermissionAssignmentService permissionAssignmentService
        )
        {
            _permissionAssignmentService = permissionAssignmentService;
        }

        [HttpPost("summaries")]
        public async Task<IActionResult> GetPermissionAssignmentSummaries()
        {
            var dtos = await _permissionAssignmentService.GetPermissionAssignmentSummariesAsync();

            var res = dtos
                .Select(x => new GetPermissionAssignmentSummariesResponseModel
                {
                    AccountId = x.AccountId,
                    Username = x.Username,

                    PermissionDepartments = x.PermissionDepartments?
                        .Select(p => new PermissionDepartmentsSummaryResponseModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,

                            Departments = p.Departments?
                                .Select(d => new DepartmentSummaryResponseModel
                                {
                                    DepartmentId = d.DepartmentId,
                                    DepartmentName = d.DepartmentName,
                                    ValidFrom = d.ValidFrom,
                                    ValidTo = d.ValidTo
                                })
                                .ToList()

                        })
                        .ToList()
                    })
                    .ToList();

            return Json(new ApiResponse<IEnumerable<GetPermissionAssignmentSummariesResponseModel>>
            {
                Success = true,
                Data = res
            });
        }
    }
}
