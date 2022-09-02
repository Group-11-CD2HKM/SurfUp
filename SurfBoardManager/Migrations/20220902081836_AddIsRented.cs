using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class AddIsRented : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRented",
                table: "BoardPost",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RentalPeriod",
                table: "BoardPost",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRented",
                table: "BoardPost");

            migrationBuilder.DropColumn(
                name: "RentalPeriod",
                table: "BoardPost");
        }
    }
}
