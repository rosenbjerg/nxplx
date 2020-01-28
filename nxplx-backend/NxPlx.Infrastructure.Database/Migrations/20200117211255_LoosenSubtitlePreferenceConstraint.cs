using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class LoosenSubtitlePreferenceConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubtitlePreferences_EpisodeFiles_FileId",
                table: "SubtitlePreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_SubtitlePreferences_FilmFiles_FileId",
                table: "SubtitlePreferences");

            migrationBuilder.DropIndex(
                name: "IX_SubtitlePreferences_FileId",
                table: "SubtitlePreferences");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePreferences_FileId",
                table: "SubtitlePreferences",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubtitlePreferences_EpisodeFiles_FileId",
                table: "SubtitlePreferences",
                column: "FileId",
                principalTable: "EpisodeFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubtitlePreferences_FilmFiles_FileId",
                table: "SubtitlePreferences",
                column: "FileId",
                principalTable: "FilmFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
