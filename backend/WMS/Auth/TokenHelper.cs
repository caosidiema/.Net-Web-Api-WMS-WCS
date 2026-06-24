using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MiddlewareDemo.Auth;

/// <summary>
/// JWT Token 生成工具
/// 参考：C:\解密文件\后端\Zocono.WMS.WebApi\App_Start\02 Auth\TokenHelp.cs
/// </summary>
public class TokenHelper
{
    private readonly JwtSettings _jwtSettings;

    public TokenHelper(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户名</param>
    /// <returns>JWT Token 字符串</returns>
    public string GenerateToken(string userId, string userName)
    {
        // 1. 创建 Claims（声明）— 存储用户信息
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 2. 创建签名密钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. 创建 Token
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials
        );

        // 4. 返回 Token 字符串
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
