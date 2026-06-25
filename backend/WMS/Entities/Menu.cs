using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiddlewareDemo.Entities;

/// <summary>
/// 菜单实体 — 对应数据库 Menus 表
/// </summary>
[Table("Menus")]
public class Menu
{
    /// <summary>
    /// 菜单ID（主键，自增）
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 父菜单ID（null 表示顶级菜单）
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称（对应 Element Plus 图标）
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
