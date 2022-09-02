using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class RentalDateEndNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalPeriod",
                table: "BoardPost");

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalDateEnd",
                table: "BoardPost",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalDateEnd",
                table: "BoardPost");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RentalPeriod",
                table: "BoardPost",
                type: "time",
                nullable: true);
        }
    }
}
