namespace MiddlewareDemo.Helper;

/// <summary>
/// 简单的日志帮助类，封装 NLog 的常用操作
/// 参考：C:\解密文件\后端\Zocono.Common\Helper\Logger.cs
/// </summary>
public static class Logger
{
    // 默认 logger（用当前类名作为 logger 名称）
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 记录普通信息日志
    /// </summary>
    public static void Info(string message)
    {
        try
        {
            _logger.Info(message);
        }
        catch { }
    }

    /// <summary>
    /// 记录调试日志
    /// </summary>
    public static void Debug(string message)
    {
        try
        {
            _logger.Debug(message);
        }
        catch { }
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    public static void Warn(string message)
    {
        try
        {
            _logger.Warn(message);
        }
        catch { }
    }

    /// <summary>
    /// 记录错误日志（带异常对象）
    /// </summary>
    public static void Error(Exception ex)
    {
        try
        {
            _logger.Error(ex);
        }
        catch { }
    }

    /// <summary>
    /// 记录错误日志（带消息 + 异常）
    /// </summary>
    public static void Error(string message, Exception ex)
    {
        try
        {
            _logger.Error(ex, message);
        }
        catch { }
    }
}
