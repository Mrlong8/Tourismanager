
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    public partial class UpdateChatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SenderAccountId và SentAt đã tồn tại trong database.
            // Không thêm cột lần nữa.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không xóa các cột đang tồn tại.
        }
    }
}
