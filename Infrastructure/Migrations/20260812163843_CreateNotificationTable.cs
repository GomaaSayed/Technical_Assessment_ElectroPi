using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOVDNjygd9Hd9i/qveZJKufsTPBZ8f3pIdUOrKjVF0rLhwTEV6Ws3xB70NbWQ3WVZQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPuxZ8C+pXqEUktB+tw9aKag3ivcVakF3FKf/zS8+SJcCvJ6f10JJJ4Y96hkYEPK7g==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "cccccccc-cccc-cccc-cccc-cccccccccccc",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENaqt4qFYkcS17FP88IU7GDfOyLRRlsx4ja1e7eomv487a1vLokzbsByKqmtYYGkOQ==");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMX6f0mB3Y7ZsqSOxpPXiMTVZsLENXbgVYKmrbn2DfyB9PzbRNye8qlDAikGIOEc1A==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJaf+zppST0Nuz01VJ1TBlpZB3ndidYaR6XisX3kaIe3Vsq11PkZWky/HRRBKbn4hw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "cccccccc-cccc-cccc-cccc-cccccccccccc",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEKLjOFPPn9+NToaGK4UZFmirZn92lgHHeHL+qwIWjRCah0ADm93JEQSabpTr6zHpw==");
        }
    }
}
