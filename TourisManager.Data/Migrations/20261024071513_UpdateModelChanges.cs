using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderAccountId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Chat");
        }
    }
}
