using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardManager.Migrations
{
    public partial class FixPropertyNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_AspNetUsers_RentedById",
                table: "BoardPost");

            migrationBuilder.RenameColumn(
                name: "RentedById",
                table: "BoardPost",
                newName: "SurfUpUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BoardPost_RentedById",
                table: "BoardPost",
                newName: "IX_BoardPost_SurfUpUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_AspNetUsers_SurfUpUserId",
                table: "BoardPost",
                column: "SurfUpUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_AspNetUsers_SurfUpUserId",
                table: "BoardPost");

            migrationBuilder.RenameColumn(
                name: "SurfUpUserId",
                table: "BoardPost",
                newName: "RentedById");

            migrationBuilder.RenameIndex(
                name: "IX_BoardPost_SurfUpUserId",
                table: "BoardPost",
                newName: "IX_BoardPost_RentedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_AspNetUsers_RentedById",
                table: "BoardPost",
                column: "RentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
