using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addadmincreatorusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc), "System Admin", "admin@imageapp.local", "$2a$11$2htlTfx2sQeWUm6uLvKjo.xFUR/gJKG5zfPqOQx7YGx8prr/tqkqy", 0 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Default Creator", "creator@imageapp.local", "$2a$11$mMr8LQPWApDDCrATE8VOeOgINLr2tGfy7sXRskd.fjIgTkp1Va.V2", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
