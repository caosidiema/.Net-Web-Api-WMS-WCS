using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiddlewareDemo.Auth;
using MiddlewareDemo.Helper;
using MiddlewareDemo.Models;

namespace MiddlewareDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenHelper _tokenHelper;
    private readonly JwtSettings _jwtSettings;

    public AuthController(TokenHelper tokenHelper, IOptions<JwtSettings> jwtSettings)
    {
        _tokenHelper = tokenHelper;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// 登录接口
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        Logger.Info($"登录请求: 用户名={request.UserName}");

        // 简单验证（实际项目应查数据库）
        // 测试账号: admin / 123456
        if (request.UserName == "admin" && request.Password == "123456")
        {
            // 生成 Token
            var token = _tokenHelper.GenerateToken("1", request.UserName);

            Logger.Info($"登录成功: {request.UserName}");

            return Ok(new LoginResponse
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.ExpireMinutes * 60
            });
        }

        Logger.Warn($"登录失败: {request.UserName} 用户名或密码错误");

        return Unauthorized(new { message = "用户名或密码错误" });
    }

    /// <summary>
    /// 测试受保护的接口
    /// GET /api/auth/profile
    /// 需要携带 JWT Token 才能访问
    /// </summary>
    [HttpGet("profile")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult GetProfile()
    {
        // 从 Token 中提取用户信息
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Ok(new
        {
            userId,
            userName,
            message = "你已通过认证，这是受保护的数据"
        });
    }
}
