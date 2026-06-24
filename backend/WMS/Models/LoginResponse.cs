namespace MiddlewareDemo.Models;

/// <summary>
/// 登录响应模型
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token 类型
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    public int ExpiresIn { get; set; }
}
