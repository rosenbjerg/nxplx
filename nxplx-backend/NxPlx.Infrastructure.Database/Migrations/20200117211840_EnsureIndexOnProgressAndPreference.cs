using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class EnsureIndexOnProgressAndPreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WatchingProgresses_FileId",
                table: "WatchingProgresses",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchingProgresses_UserId",
                table: "WatchingProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePreferences_FileId",
                table: "SubtitlePreferences",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePreferences_UserId",
                table: "SubtitlePreferences",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatchingProgresses_FileId",
                table: "WatchingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_WatchingProgresses_UserId",
                table: "WatchingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_SubtitlePreferences_FileId",
                table: "SubtitlePreferences");

            migrationBuilder.DropIndex(
                name: "IX_SubtitlePreferences_UserId",
                table: "SubtitlePreferences");
        }
    }
}
