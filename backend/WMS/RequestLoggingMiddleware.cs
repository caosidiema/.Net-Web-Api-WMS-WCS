// RequestLoggingMiddleware.cs
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("--> 请求: {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        // 调用下一个中间件
        await _next(context);

        // 响应返回时执行
        _logger.LogInformation("<-- 响应状态: {StatusCode}", context.Response.StatusCode);
    }
}