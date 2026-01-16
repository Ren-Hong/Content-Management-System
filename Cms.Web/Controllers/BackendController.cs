using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    public class BackendController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
