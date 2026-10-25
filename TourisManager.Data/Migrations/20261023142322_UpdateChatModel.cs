
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    public partial class UpdateChatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hai cột SenderAccountId và SentAt đã tồn tại trong database.
            // Không cần thêm cột lần nữa.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không xóa hai cột vì chúng đã tồn tại trước migration này.
        }
    }
}
