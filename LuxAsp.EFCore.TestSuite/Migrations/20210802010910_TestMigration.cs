using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LuxAsp.EFCore.TestSuite.Migrations
{
    public partial class TestMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LuxDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(36)", nullable: false),
                    CategoryId = table.Column<string>(type: "varchar(36)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastWriteTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastStateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuxDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LuxTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LatestRefreshTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RefreshId = table.Column<string>(type: "varchar(36)", nullable: false),
                    RefreshExpiration = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuxTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LuxUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastWriteTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LoginName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    PasswordTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuxUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "user_by_token",
                table: "LuxTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LuxUsers_LoginName",
                table: "LuxUsers",
                column: "LoginName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LuxDocuments");

            migrationBuilder.DropTable(
                name: "LuxTokens");

            migrationBuilder.DropTable(
                name: "LuxUsers");
        }
    }
}
