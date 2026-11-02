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
            // Tạo cột SenderAccountId trong bảng Chat
            // Cột cho phép NULL vì tin nhắn hệ thống có thể không có tài khoản gửi
            migrationBuilder.AddColumn<string>(
                name: "SenderAccountId",
                table: "Chat",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true);

            // Tạo chỉ mục cho SenderAccountId
            migrationBuilder.CreateIndex(
                name: "IX_Chat_SenderAccountId",
                table: "Chat",
                column: "SenderAccountId");

            // Tạo khóa ngoại từ Chat sang Account
            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Account_SenderAccountId",
                table: "Chat",
                column: "SenderAccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Account_SenderAccountId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_SenderAccountId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "SenderAccountId",
                table: "Chat");
        }
    }
}