using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MiddlewareDemo.Auth;

/// <summary>
/// 向 OpenAPI 文档添加 JWT Bearer 安全方案
/// 适配 Microsoft.OpenApi 2.0.0（.NET 10）
/// </summary>
public class JwtSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // 1. 定义 JWT Bearer 安全方案
        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "请输入 JWT Token（不需要加 Bearer 前缀）",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        // 2. 添加到 Components.SecuritySchemes
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = securityScheme;

        // 3. 创建引用和安全要求
        var reference = new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document);
        var securityRequirement = new OpenApiSecurityRequirement
        {
            { reference, new List<string>() }
        };

        // 4. 设置到文档级别
        document.Security = new List<OpenApiSecurityRequirement> { securityRequirement };

        return Task.CompletedTask;
    }
}
