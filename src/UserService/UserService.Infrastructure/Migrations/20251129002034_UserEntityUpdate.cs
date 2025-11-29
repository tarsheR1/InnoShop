using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3f60860c-f265-40b1-9160-6f51d3b7867f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44ccda16-53b1-452d-b946-acc6341aca56"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("7250f59b-a2a9-4bb6-abe0-c1c67de30c7c"));

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpires",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3f60860c-f265-40b1-9160-6f51d3b7867f"), new DateTime(2025, 11, 27, 23, 42, 19, 600, DateTimeKind.Utc).AddTicks(1206), "Администратор системы", "Admin", null },
                    { new Guid("44ccda16-53b1-452d-b946-acc6341aca56"), new DateTime(2025, 11, 27, 23, 42, 19, 600, DateTimeKind.Utc).AddTicks(2595), "Модератор", "Moderator", null },
                    { new Guid("7250f59b-a2a9-4bb6-abe0-c1c67de30c7c"), new DateTime(2025, 11, 27, 23, 42, 19, 600, DateTimeKind.Utc).AddTicks(2707), "Обычный пользователь", "User", null }
                });
        }
    }
}
