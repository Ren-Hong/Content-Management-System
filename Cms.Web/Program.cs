using Cms.Application.Services.Account;
using Cms.Infrastructure.Repositories.Account;
using Cms.Infrastructure.Repositories.UnitOfWork;
using Cms.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        // 前端 JSON 中的 enum 字串值會由 System.Text.Json 自動轉成對應的 enum
        // 讓 ApiController 的 RequestModel 可以寫 Enum 
        o.JsonSerializerOptions.Converters.Add( 
            new JsonStringEnumConverter()
        );
    });


// 自動 DI 註冊, 請先安裝 Scrutor 套件
builder.Services.Scan(scan => scan
    .FromAssemblies(
        typeof(Program).Assembly,           // Cms.Web
        typeof(IAccountService).Assembly,   // Cms.Application
        typeof(IAccountRepository).Assembly // Cms.Infrastructure
    )
    .AddClasses(c => c.Where(t =>
        t.Name.EndsWith("Service") || //只要尾巴叫 Service 都會被 DI new
        t.Name.EndsWith("Repository")))
    .AsImplementedInterfaces() // 請人資用職稱找人過來, 不要直接用姓名找人, 如果有兩個人都是相同職稱, 人資會找最後登記的那位(最後Assembly到的那位)
    .WithScopedLifetime() //scoped生命週期
);

// 註冊 DB Connection String
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Service 層要包 Repository 交易
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 註冊登入章
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //我們系統的預設登入機制是 "Cookie"
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; //如果某個頁面需要登入，但使用者還沒登入 → 自動導到這個網址
        options.AccessDeniedPath = "/Account/AccessDenied"; // 已經登入了，但「權限不夠」→ 自動導到這個網址
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // 這個登入章（Cookie）多久會失效
        options.SlidingExpiration = true; // 只要使用者「持續在使用系統」Cookie 就會自動延長
    });

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()              // Info 以上才記
    .WriteTo.Console()                        // 寫到 console
    .WriteTo.File(
        path: "logs/app-.log",                // logs 資料夾
        rollingInterval: RollingInterval.Day, // 每天一個檔
        retainedFileCountLimit: 14             // 留 14 天
    )
    .CreateLogger();

// 告訴 ASP.NET Core：ILogger 改用 Serilog
builder.Host.UseSerilog();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // 一定要在 Authorization 前

app.UseAuthorization();

// 起站位置
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}" //當使用者沒指定Controller且action也沒指定的時候自動跳轉這個頁面
);

app.Run();
