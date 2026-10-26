
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    public partial class UpdateModelChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Không thêm cột vì SenderAccountId và SentAt đã tồn tại.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không xóa các cột đã tồn tại.
        }
    }
}
