namespace MiddlewareDemo.Models;

/// <summary>
/// 菜单 DTO — 返回给前端的树形菜单结构
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public List<MenuDto>? Children { get; set; }
}
