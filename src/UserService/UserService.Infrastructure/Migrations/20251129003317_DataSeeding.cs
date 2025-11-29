using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("358da0e8-fb33-4d1b-ab3d-3ad670d4c63e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("5392abc3-a6d7-4b51-8324-3c33cd489880"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("585360f1-f5ac-4698-8c7d-b73ab634a983"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 11, 29, 0, 33, 16, 636, DateTimeKind.Utc).AddTicks(1233), "Администратор системы", "Admin", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 11, 29, 0, 33, 16, 636, DateTimeKind.Utc).AddTicks(2702), "Модератор", "Moderator", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 11, 29, 0, 33, 16, 636, DateTimeKind.Utc).AddTicks(2716), "Обычный пользователь", "User", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("358da0e8-fb33-4d1b-ab3d-3ad670d4c63e"), new DateTime(2025, 11, 29, 0, 29, 24, 665, DateTimeKind.Utc).AddTicks(3666), "Администратор системы", "Admin", null },
                    { new Guid("5392abc3-a6d7-4b51-8324-3c33cd489880"), new DateTime(2025, 11, 29, 0, 29, 24, 665, DateTimeKind.Utc).AddTicks(5303), "Обычный пользователь", "User", null },
                    { new Guid("585360f1-f5ac-4698-8c7d-b73ab634a983"), new DateTime(2025, 11, 29, 0, 29, 24, 665, DateTimeKind.Utc).AddTicks(5244), "Модератор", "Moderator", null }
                });
        }
    }
}
