using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("05a30226-d959-412a-96b1-fd76c0c69dec"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("583d700d-d7fd-4d09-b5cd-3d22b7a438d8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("5d9779da-34ab-45a3-9329-1522a8bc15ce"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("05a30226-d959-412a-96b1-fd76c0c69dec"), new DateTime(2025, 11, 29, 0, 20, 30, 754, DateTimeKind.Utc).AddTicks(1822), "Модератор", "Moderator", null },
                    { new Guid("583d700d-d7fd-4d09-b5cd-3d22b7a438d8"), new DateTime(2025, 11, 29, 0, 20, 30, 754, DateTimeKind.Utc).AddTicks(368), "Администратор системы", "Admin", null },
                    { new Guid("5d9779da-34ab-45a3-9329-1522a8bc15ce"), new DateTime(2025, 11, 29, 0, 20, 30, 754, DateTimeKind.Utc).AddTicks(1832), "Обычный пользователь", "User", null }
                });
        }
    }
}
