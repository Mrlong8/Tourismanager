using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    public partial class UpdateChatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bổ sung lệnh tạo cột để đồng bộ cho tất cả các máy và server
    
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Lệnh khôi phục (xóa cột) nếu cần rollback
            migrationBuilder.DropColumn(
                name: "SenderAccountId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Chat");
        }
    }
}