using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    public class LoginController : Controller
    {
        // 顯示登入頁
        [HttpGet]
        public IActionResult Index()
        {
            // 已登入
            if (User.Identity?.IsAuthenticated == true)
            {
                // Admin → 後台
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Backend");
                }

                // 其他登入者 → 前台
                return RedirectToAction("Index", "Frontend");
            }

            // 尚未登入 → 顯示登入頁
            return View();
        }
    }
}
