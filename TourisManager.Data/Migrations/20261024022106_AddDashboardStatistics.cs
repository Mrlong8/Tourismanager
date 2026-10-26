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
            // Đồng bộ kiểu dữ liệu với Account.AccountId: varchar(36).
            // Giữ nullable để hỗ trợ tin nhắn tự động.
            migrationBuilder.AlterColumn<string>(
                name: "SenderAccountId",
                table: "Chat",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

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
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Account_SenderAccountId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_SenderAccountId",
                table: "Chat");
        }
    }
}
