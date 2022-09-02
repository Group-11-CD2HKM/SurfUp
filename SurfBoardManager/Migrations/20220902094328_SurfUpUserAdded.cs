using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class SurfUpUserAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RentedById",
                table: "BoardPost",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SurfUpUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfUpUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardPost_RentedById",
                table: "BoardPost",
                column: "RentedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_SurfUpUser_RentedById",
                table: "BoardPost",
                column: "RentedById",
                principalTable: "SurfUpUser",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_SurfUpUser_RentedById",
                table: "BoardPost");

            migrationBuilder.DropTable(
                name: "SurfUpUser");

            migrationBuilder.DropIndex(
                name: "IX_BoardPost_RentedById",
                table: "BoardPost");

            migrationBuilder.DropColumn(
                name: "RentedById",
                table: "BoardPost");
        }
    }
}
