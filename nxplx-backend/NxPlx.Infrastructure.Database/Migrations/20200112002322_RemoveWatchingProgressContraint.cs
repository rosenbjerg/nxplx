using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class RemoveWatchingProgressContraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchingProgresses_EpisodeFiles_FileId",
                table: "WatchingProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchingProgresses_FilmFiles_FileId",
                table: "WatchingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_WatchingProgresses_FileId",
                table: "WatchingProgresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WatchingProgresses_FileId",
                table: "WatchingProgresses",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchingProgresses_EpisodeFiles_FileId",
                table: "WatchingProgresses",
                column: "FileId",
                principalTable: "EpisodeFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchingProgresses_FilmFiles_FileId",
                table: "WatchingProgresses",
                column: "FileId",
                principalTable: "FilmFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
