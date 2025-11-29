using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChange3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 11, 29, 0, 34, 59, 87, DateTimeKind.Utc).AddTicks(9031), "Администратор системы", "Admin", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 11, 29, 0, 34, 59, 88, DateTimeKind.Utc).AddTicks(718), "Модератор", "Moderator", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 11, 29, 0, 34, 59, 88, DateTimeKind.Utc).AddTicks(732), "Обычный пользователь", "User", null }
                });
        }
    }
}
