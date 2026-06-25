using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiddlewareDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreateTime", "Icon", "Name", "ParentId", "Path", "SortOrder" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "DataBoard", "首页", null, "/home", 1 },
                    { 2, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Box", "库存管理", null, "/inventory", 2 },
                    { 3, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "ShoppingCart", "出入库管理", null, "/flow", 3 },
                    { 4, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Setting", "系统管理", null, "/system", 4 },
                    { 5, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Box", "库存查询", 2, "/inventory/query", 1 },
                    { 6, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Goods", "库存预警", 2, "/inventory/warning", 2 },
                    { 7, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "List", "库存盘点", 2, "/inventory/check", 3 },
                    { 8, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Download", "入库管理", 3, "/inbound/list", 1 },
                    { 9, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Upload", "出库管理", 3, "/outbound/list", 2 },
                    { 10, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Location", "出入库记录", 3, "/flow/record", 3 },
                    { 11, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "User", "用户管理", 4, "/system/users", 1 },
                    { 12, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Avatar", "角色管理", 4, "/system/roles", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
