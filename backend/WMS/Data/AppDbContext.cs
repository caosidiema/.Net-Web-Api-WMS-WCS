using Microsoft.EntityFrameworkCore;
using MiddlewareDemo.Entities;

namespace MiddlewareDemo.Data
{
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

        /// <summary>
        /// Menus 表
        /// </summary>
        public DbSet<Menu> Menus { get; set; }

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
                Password = "123456",
                Name = "管理员",
                CreateTime = new DateTime(2026, 6, 9)
            });

            // 菜单种子数据
            var menuTime = new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 1, ParentId = null, Name = "首页", Path = "/home", Icon = "DataBoard", SortOrder = 1, CreateTime = menuTime },
                new Menu { Id = 2, ParentId = null, Name = "库存管理", Path = "/home/inventory", Icon = "Box", SortOrder = 2, CreateTime = menuTime },
                new Menu { Id = 3, ParentId = null, Name = "出入库管理", Path = "/home/flow", Icon = "ShoppingCart", SortOrder = 3, CreateTime = menuTime },
                new Menu { Id = 4, ParentId = null, Name = "系统管理", Path = "/home/system", Icon = "Setting", SortOrder = 4, CreateTime = menuTime },
                new Menu { Id = 5, ParentId = 2, Name = "库存查询", Path = "/home/inventory/query", Icon = "Box", SortOrder = 1, CreateTime = menuTime },
                new Menu { Id = 6, ParentId = 2, Name = "库存预警", Path = "/home/inventory/warning", Icon = "Goods", SortOrder = 2, CreateTime = menuTime },
                new Menu { Id = 7, ParentId = 2, Name = "库存盘点", Path = "/home/inventory/check", Icon = "List", SortOrder = 3, CreateTime = menuTime },
                new Menu { Id = 8, ParentId = 3, Name = "入库管理", Path = "/home/inbound/list", Icon = "Download", SortOrder = 1, CreateTime = menuTime },
                new Menu { Id = 9, ParentId = 3, Name = "出库管理", Path = "/home/outbound/list", Icon = "Upload", SortOrder = 2, CreateTime = menuTime },
                new Menu { Id = 10, ParentId = 3, Name = "出入库记录", Path = "/home/flow/record", Icon = "Location", SortOrder = 3, CreateTime = menuTime },
                new Menu { Id = 11, ParentId = 4, Name = "用户管理", Path = "/home/system/users", Icon = "User", SortOrder = 1, CreateTime = menuTime },
                new Menu { Id = 12, ParentId = 4, Name = "角色管理", Path = "/home/system/roles", Icon = "Avatar", SortOrder = 2, CreateTime = menuTime }
            );
        }
    }
}
