using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class AddedBoardImageAndTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BoardImage",
                table: "BoardPost",
                type: "NVarChar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BoardImage",
                table: "BoardPost",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVarChar(255)");
        }
    }
}
