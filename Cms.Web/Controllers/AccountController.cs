using Cms.Application.Services.Account;
using Cms.Application.Services.Account.Dtos;
using Cms.Web.Models.Account;
using Cms.Web.Models.Api;
using Cms.Web.Models.username;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers
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

        // 處理登入
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ValidationErrors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                        )
                });
            }

            var dto = new LoginRequestDto
            {
                Username = req.Username,
                Password = req.Password
            };

            var rdto = await _accountService.LoginAsync(dto);

            if (rdto.Result != LoginResult.Success)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ErrorCode = rdto.Result.ToString()
                });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, rdto.AccountId!.Value.ToString()),
                new Claim(ClaimTypes.Name, rdto.Username!)
            };

            if (rdto.Roles != null)
            {
                foreach (var role in rdto.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (rdto.Permissions != null)
            {
                foreach (var permission in rdto.Permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return Json(new ApiResponse<LoginResponseModel>
            {
                Success = true,
                Data = new LoginResponseModel{
                    IsAdmin = rdto.Roles?.Contains("Admin") == true //左邊必須是true才是true, null或false則為false
                }
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountSummaries()
        {
            var rdto = await _accountService.GetAccountSummariesAsync();

            var res = rdto.Select(x => new GetAccountSummariesResponseModel // 跟 dto 長依樣
            {
                Username = x.Username,
                Status = x.Status,
                Roles = x.Roles
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetAccountSummariesResponseModel>>
            {
                Success = true,
                Data = res 
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse
                {
                    Success = false,
                    ValidationErrors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                        )
                });
            }

            var rdto = await _accountService.CreateAccountAsync(
                new CreateAccountRequestDto
                {
                    Username = req.Username,
                    Password = req.Password,
                    RoleCode = req.RoleCode
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == CreateAccountResult.Success,
                ErrorCode = rdto.Result != CreateAccountResult.Success
                    ? rdto.Result.ToString() // 這邊要ToString喔, 不然前端會拿到魔法數字
                    : null
            });
        }
    }
}
