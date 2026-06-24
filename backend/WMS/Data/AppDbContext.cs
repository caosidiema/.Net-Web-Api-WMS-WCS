using Microsoft.EntityFrameworkCore;
using MiddlewareDemo.Entities;

namespace MiddlewareDemo.Data;

/// <summary>
/// EF Core 数据库上下文 — 管理数据库连接和表映射
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Users 表
    /// </summary>
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 UserName 唯一索引
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        // 插入一条默认测试数据（密码: 123456）
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            UserName = "admin",
            Password = "123456",  // 实际项目应加密存储
            Name = "管理员",
            CreateTime = new DateTime(2026, 6, 9)
        });
    }
}
