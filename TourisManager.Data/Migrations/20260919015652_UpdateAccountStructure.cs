using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_User_UserId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_CreatorUserId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_User_CreatorUserId",
                table: "Restaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Review_UserId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_UserId",
                table: "Favorite");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Restaurant",
                newName: "CreatorAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurant_CreatorUserId",
                table: "Restaurant",
                newName: "IX_Restaurant_CreatorAccountId");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Location",
                newName: "CreatorAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_CreatorUserId",
                table: "Location",
                newName: "IX_Location_CreatorAccountId");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Review",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Favorite",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvataUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_AccountId",
                table: "Review",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_AccountId",
                table: "Favorite",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Account_AccountId",
                table: "Favorite",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Account_CreatorAccountId",
                table: "Location",
                column: "CreatorAccountId",
                principalTable: "Account",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Account_CreatorAccountId",
                table: "Restaurant",
                column: "CreatorAccountId",
                principalTable: "Account",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Account_AccountId",
                table: "Review",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Account_AccountId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Account_CreatorAccountId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Account_CreatorAccountId",
                table: "Restaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Account_AccountId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Review_AccountId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_AccountId",
                table: "Favorite");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Favorite");

            migrationBuilder.RenameColumn(
                name: "CreatorAccountId",
                table: "Restaurant",
                newName: "CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurant_CreatorAccountId",
                table: "Restaurant",
                newName: "IX_Restaurant_CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorAccountId",
                table: "Location",
                newName: "CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_CreatorAccountId",
                table: "Location",
                newName: "IX_Location_CreatorUserId");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    AuthProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvataUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_UserId",
                table: "Favorite",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_User_UserId",
                table: "Favorite",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_CreatorUserId",
                table: "Location",
                column: "CreatorUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_User_CreatorUserId",
                table: "Restaurant",
                column: "CreatorUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
