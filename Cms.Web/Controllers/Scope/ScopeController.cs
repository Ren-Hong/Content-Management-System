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
    }

}

