using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class NullAble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_SurfUpUser_RentedById",
                table: "BoardPost");

            migrationBuilder.DropTable(
                name: "SurfUpUser");

            migrationBuilder.AlterColumn<string>(
                name: "Equipment",
                table: "BoardPost",
                type: "NVarChar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVarChar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "BoardImage",
                table: "BoardPost",
                type: "NVarChar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVarChar(255)");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_AspNetUsers_RentedById",
                table: "BoardPost",
                column: "RentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_AspNetUsers_RentedById",
                table: "BoardPost");

            migrationBuilder.AlterColumn<string>(
                name: "Equipment",
                table: "BoardPost",
                type: "NVarChar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVarChar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BoardImage",
                table: "BoardPost",
                type: "NVarChar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVarChar(255)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SurfUpUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfUpUser", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_SurfUpUser_RentedById",
                table: "BoardPost",
                column: "RentedById",
                principalTable: "SurfUpUser",
                principalColumn: "Id");
        }
    }
}
