using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class RemoveMediaFileBaseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaDetailsId",
                table: "FilmFiles");

            migrationBuilder.DropColumn(
                name: "MediaDetailsId",
                table: "EpisodeFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaDetailsId",
                table: "FilmFiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaDetailsId",
                table: "EpisodeFiles",
                type: "integer",
                nullable: true);
        }
    }
}
