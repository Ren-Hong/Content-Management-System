using Cms.Contract.Common.Pagination;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Services.Account.Dtos;
using Cms.Contract.Services.Account.Interfaces;
using Cms.Web.Controllers.Account.Models;
using Cms.Web.Controllers.Department.Models;
using Cms.Web.Controllers.Role.Models;
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
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
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

            if (rdto.RoleCodes != null)
            {
                foreach (var roleCode in rdto.RoleCodes)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleCode));
                }
            }

            if (rdto.PermissionCodes != null)
            {
                foreach (var permissionCode in rdto.PermissionCodes)
                {
                    claims.Add(new Claim("permission", permissionCode));
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
                    IsAdmin = rdto.RoleCodes?.Contains("Admin") == true //左邊必須是true才是true, null或false則為false
                }
            });
        }

        
        [HttpPost("summaries")]
        public async Task<IActionResult> GetAccountSummaries([FromBody] PageRequest req)
        {
            var rdto = await _accountService.GetAccountSummariesAsync(req);

            var res = new PagedResult<GetAccountSummariesResponseModel>
            {
                Page = rdto.Page,
                PageSize = rdto.PageSize,
                TotalCount = rdto.TotalCount,

                Items = rdto.Items.Select(x => new GetAccountSummariesResponseModel
                {
                    AccountId = x.AccountId,
                    Username = x.Username,
                    Status = x.Status,

                    Departments = x.Departments.Select(d => new GetDepartmentOptionsResponseModel
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.DepartmentName
                    }).ToList(),

                    Roles = x.Roles.Select(r => new GetRoleOptionsResponseModel
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName
                    }).ToList()

                }).ToList()
            };

            return Json(new ApiResponse<PagedResult<GetAccountSummariesResponseModel>>
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
                    RoleIds = req.RoleIds
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
                    RoleIds = req.RoleIds,
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

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel req)
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

            var rdto = await _accountService.ResetPasswordAsync(
                new ResetPasswordRequestDto
                {
                    Username = req.Username,
                    Password = req.Password,
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == ResetPasswordResult.Success,
                ErrorCode = rdto.Result != ResetPasswordResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequestModel req)
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

            var rdto = await _accountService.DeleteAccountAsync(
                new DeleteAccountRequestDto
                {
                    Username = req.Username,
                }
            );

            return Json(new ApiResponse
            {
                Success = rdto.Result == DeleteAccountResult.Success,
                ErrorCode = rdto.Result != DeleteAccountResult.Success
                    ? rdto.Result.ToString()
                    : null
            });
        }

    }
}
