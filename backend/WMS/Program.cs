using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiddlewareDemo.Auth;
using MiddlewareDemo.Data;
using MiddlewareDemo.Helper;
using NLog.Web;
using Scalar.AspNetCore;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// 添加 OpenAPI 服务
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<JwtSecuritySchemeTransformer>();
});

// 添加控制器服务
builder.Services.AddControllers();
// JWT 认证配置                                                                                                       
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenHelper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseCors("AllowAll");

// 映射 OpenAPI 端点（生成 /openapi/v1.json）
app.MapOpenApi();

// 映射 Scalar UI（访问 /scalar/v1 查看 API 文档）
app.MapScalarApiReference();
app.UseAuthentication();
app.UseAuthorization();
// 映射控制器端点
app.MapControllers();

// 1. 异常处理 - 放在最外层
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Logger.Error("请求处理出错", ex);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"出错啦: {ex.Message}");
    }
});

// 2. 请求日志中间件
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. 性能计时中间件
app.UseMiddleware<PerformanceMiddleware>();

// 4. 一个短路中间件（如果请求路径是 /ping，直接返回 Pong，不再往下走）
app.Map("/ping", pongApp =>
{
    pongApp.Run(async context =>
    {
        await context.Response.WriteAsync("Pong!");
    });
});

// 5. 普通终端中间件 - 处理正常请求
app.Map("/hello", helloApp =>
{
    helloApp.Run(async context =>
    {
        await context.Response.WriteAsync("Hello, World!");
    });
});

// 默认回落处理 - 使用 Use 而非 Run，让请求能走到端点路由
app.Use(async (context, next) =>
{
    await next();
    // 如果没有任何端点处理了这个请求（比如 /openapi/v1.json），才返回 404
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        await context.Response.WriteAsync("Not Found");
    }
});

app.Run();