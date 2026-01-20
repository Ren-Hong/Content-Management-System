using Cms.Application.Services.Account;
using Cms.Application.Services.Account.Dtos;
using Cms.Application.Services.Domain;
using Cms.Web.Controllers.Account.Models;
using Cms.Web.Controllers.Contracts.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // 顯示登入頁
        [HttpGet]
        public IActionResult Login()
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
