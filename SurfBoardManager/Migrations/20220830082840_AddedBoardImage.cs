using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class AddedBoardImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoardImage",
                table: "BoardPost",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardImage",
                table: "BoardPost");
        }
    }
}
