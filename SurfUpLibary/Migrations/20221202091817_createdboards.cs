using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfUpLibary.Migrations
{
    public partial class createdboards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoardCreatorId",
                table: "BoardPost",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardPost_BoardCreatorId",
                table: "BoardPost",
                column: "BoardCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardPost_AspNetUsers_BoardCreatorId",
                table: "BoardPost",
                column: "BoardCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardPost_AspNetUsers_BoardCreatorId",
                table: "BoardPost");

            migrationBuilder.DropIndex(
                name: "IX_BoardPost_BoardCreatorId",
                table: "BoardPost");

            migrationBuilder.DropColumn(
                name: "BoardCreatorId",
                table: "BoardPost");
        }
    }
}
