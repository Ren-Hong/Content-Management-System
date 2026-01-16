using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    public class FrontendController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
