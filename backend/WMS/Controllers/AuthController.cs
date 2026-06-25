using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiddlewareDemo.Auth;
using MiddlewareDemo.Data;
using MiddlewareDemo.Helper;
using MiddlewareDemo.Models;

namespace MiddlewareDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenHelper _tokenHelper;
    private readonly JwtSettings _jwtSettings;
    private readonly AppDbContext _dbContext;

    public AuthController(TokenHelper tokenHelper, IOptions<JwtSettings> jwtSettings, AppDbContext dbContext)
    {
        _tokenHelper = tokenHelper;
        _jwtSettings = jwtSettings.Value;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 登录接口
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Logger.Info($"登录请求: 用户名={request.UserName}");

        // 从数据库验证用户
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.Password == request.Password);

        if (user != null)
        {
            // 生成 Token
            var token = _tokenHelper.GenerateToken(user.Id.ToString(), user.UserName);

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
    /// 获取用户信息（需要 Token）
    /// GET /api/auth/profile
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

    /// <summary>
    /// 获取动态菜单（需要 Token）
    /// GET /api/auth/menus
    /// </summary>
    [HttpGet("menus")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetMenus()
    {
        // 从数据库查询所有菜单，按排序号排列
        var allMenus = await _dbContext.Menus
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        // 构建树形结构：找出顶级菜单，然后挂载子菜单
        var topLevel = allMenus.Where(m => m.ParentId == null).ToList();
        var result = topLevel.Select(parent => new MenuDto
        {
            Name = parent.Name,
            Path = parent.Path,
            Icon = parent.Icon,
            Children = allMenus
                .Where(m => m.ParentId == parent.Id)
                .Select(child => new MenuDto
                {
                    Name = child.Name,
                    Path = child.Path,
                    Icon = child.Icon
                })
                .ToList()
        }).ToList();

        return Ok(result);
    }
}
