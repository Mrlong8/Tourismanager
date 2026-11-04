using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGoogleMapUrlToCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapUrl",
                table: "DetailAccounts");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "DetailAccounts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "DetailAccounts",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "DetailAccounts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "DetailAccounts");

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapUrl",
                table: "DetailAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
