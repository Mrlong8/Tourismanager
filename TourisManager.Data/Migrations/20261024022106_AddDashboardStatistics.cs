using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. TẠO CỘT TRƯỚC ĐỂ TRÁNH LỖI
            migrationBuilder.AddColumn<string>(
                name: "SenderAccountId",
                table: "Chat",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Chat",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // 2. SAU KHI CỘT ĐÃ TỒN TẠI, MỚI GẮN INDEX VÀ KHÓA NGOẠI
            migrationBuilder.CreateIndex(
                name: "IX_Chat_SenderAccountId",
                table: "Chat",
                column: "SenderAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Account_SenderAccountId",
                table: "Chat",
                column: "SenderAccountId",
                principalTable: "Account",
                principalColumn: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khi rollback thì làm ngược lại: Xóa khóa ngoại và index trước
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Account_SenderAccountId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_SenderAccountId",
                table: "Chat");

            // Sau đó mới xóa cột
            migrationBuilder.DropColumn(
                name: "SenderAccountId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Chat");
        }
    }
}