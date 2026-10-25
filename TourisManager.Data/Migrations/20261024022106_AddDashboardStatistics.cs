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
