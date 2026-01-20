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
    [ApiController]
    [Route("api/account")]
    [Authorize(Roles = "Admin")]
    public class AccountApiController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountApiController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPost("summaries")]
        public async Task<IActionResult> GetAccountSummaries()
        {
            var rdto = await _accountService.GetAccountSummariesAsync();

            var res = rdto.Select(x => new GetAccountSummariesResponseModel // 跟 dto 長依樣
            {
                Username = x.Username,
                Status = x.Status.ToString(),
                Roles = x.Roles.Select(r => new RoleResponseModel
                {
                    RoleCode = r.RoleCode,
                    RoleName = r.RoleName
                }).ToList()
            }).ToList();

            return Json(new ApiResponse<IEnumerable<GetAccountSummariesResponseModel>>
            {
                Success = true,
                Data = res 
            });
        }

        [HttpPost("create")]
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

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequestModel req)
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

            var rdto = await _accountService.UpdateAccountAsync(
                new UpdateAccountRequestDto
                {
                    Username = req.Username,
                    RoleCode = req.RoleCode,
                    Status   = req.Status,
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == UpdateAccountResult.Success,
                ErrorCode = rdto.Result != UpdateAccountResult.Success
                    ? rdto.Result.ToString() 
                    : null
            });
        }

    }
}
