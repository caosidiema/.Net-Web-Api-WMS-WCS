using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiddlewareDemo.Data;

/// <summary>
/// 设计时 DbContext 工厂 — 供 dotnet ef migrations 使用
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MiddlewareDemo;Username=postgres;Password=123456");

        return new AppDbContext(optionsBuilder.Options);
    }
}
