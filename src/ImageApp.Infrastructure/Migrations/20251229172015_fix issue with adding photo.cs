using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixissuewithaddingphoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Photos",
                keyColumn: "BlobThumbnailKey",
                keyValue: null,
                column: "BlobThumbnailKey",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "BlobThumbnailKey",
                table: "Photos",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$UtQd32rM0rl0eXWLVXey/uAsHZji/6ejpVZm5yrTNGTduzskvvGL.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$j7sKYGQO/jov5FRLBUfZteSM5yxhhyPycX2GvBXGr8FomFaIzoCOO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BlobThumbnailKey",
                table: "Photos",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$2htlTfx2sQeWUm6uLvKjo.xFUR/gJKG5zfPqOQx7YGx8prr/tqkqy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$mMr8LQPWApDDCrATE8VOeOgINLr2tGfy7sXRskd.fjIgTkp1Va.V2");
        }
    }
}
