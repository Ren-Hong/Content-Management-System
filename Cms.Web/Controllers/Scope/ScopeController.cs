using Cms.Contract.Services.Scope.Dtos;
using Cms.Contract.Services.Scope.Interfaces;
using Cms.Contract.Controllers.Api;
using Cms.Web.Controllers.Role.Models;
using Cms.Web.Controllers.Scope.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Scope
{
    [ApiController]
    [Route("api/scope")]
    [Authorize(Roles = "Admin")]
    public class ScopeController : Controller
    {
        private readonly IScopeService _scopeService;

        public ScopeController(IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        [HttpPost("options")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetScopeOptions()
        {
            var rdto = await _scopeService.GetScopeOptionsAsync();

            var res = rdto.Select(x => new GetScopeOptionsResponseModel
            {
                ScopeName = x.ScopeName,
                ScopeCode = x.ScopeCode,
                ScopeId = x.ScopeId,
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetScopeOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

    }
}

