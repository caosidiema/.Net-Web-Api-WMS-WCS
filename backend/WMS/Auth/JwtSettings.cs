namespace MiddlewareDemo.Auth;

/// <summary>
/// JWT 配置类，对应 appsettings.json 中的 "Jwt" 节点
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 签名密钥（至少 32 个字符）
    /// </summary>
    public string SecurityKey { get; set; } = "ThisIsASecretKeyForDemo123456789!";

    /// <summary>
    /// 签发者
    /// </summary>
    public string Issuer { get; set; } = "MiddlewareDemo";

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = "MiddlewareDemo";

    /// <summary>
    /// Token 过期时间（分钟）
    /// </summary>
    public int ExpireMinutes { get; set; } = 60;
}
