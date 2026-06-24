using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiddlewareDemo.Entities;

/// <summary>
/// 用户实体 — 对应数据库 Users 表
/// 参考：C:\解密文件\后端\Zocono.WMS.Domain\UserDomain\UserInfo
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 用户ID（主键，自增）
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 用户名（唯一）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(50)]
    public string? Name { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
