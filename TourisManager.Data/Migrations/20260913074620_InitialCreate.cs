using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourisManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
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
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    FavoriteId = table.Column<string>(type: "varchar(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => x.FavoriteId);
                    table.ForeignKey(
                        name: "FK_Favorite_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "varchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Location_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Location_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Restaurant",
                columns: table => new
                {
                    RestaurantId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "varchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurant", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_Restaurant_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Restaurant_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<string>(type: "varchar(36)", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetId = table.Column<string>(type: "varchar(36)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Review_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationImage",
                columns: table => new
                {
                    LocationImageId = table.Column<string>(type: "varchar(36)", nullable: false),
                    LocationId = table.Column<string>(type: "varchar(36)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationImage", x => x.LocationImageId);
                    table.ForeignKey(
                        name: "FK_LocationImage_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewContent",
                columns: table => new
                {
                    ReviewContentId = table.Column<string>(type: "varchar(36)", nullable: false),
                    ReviewId = table.Column<string>(type: "varchar(36)", nullable: false),
                    ContentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewContent", x => x.ReviewContentId);
                    table.ForeignKey(
                        name: "FK_ReviewContent_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Review",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_UserId",
                table: "Favorite",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CategoryId",
                table: "Location",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatorUserId",
                table: "Location",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationImage_LocationId",
                table: "LocationImage",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_CategoryId",
                table: "Restaurant",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_CreatorUserId",
                table: "Restaurant",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewContent_ReviewId",
                table: "ReviewContent",
                column: "ReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "LocationImage");

            migrationBuilder.DropTable(
                name: "Restaurant");

            migrationBuilder.DropTable(
                name: "ReviewContent");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
