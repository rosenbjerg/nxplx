using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class RemakeCompositeKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchingProgresses",
                table: "WatchingProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitlePreferences",
                table: "SubtitlePreferences");

            migrationBuilder.AlterColumn<int>(
                name: "SeasonDetailsId",
                table: "EpisodeDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchingProgresses",
                table: "WatchingProgresses",
                columns: new[] { "UserId", "FileId", "MediaType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitlePreferences",
                table: "SubtitlePreferences",
                columns: new[] { "UserId", "FileId", "MediaType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchingProgresses",
                table: "WatchingProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitlePreferences",
                table: "SubtitlePreferences");

            migrationBuilder.AlterColumn<int>(
                name: "SeasonDetailsId",
                table: "EpisodeDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchingProgresses",
                table: "WatchingProgresses",
                columns: new[] { "UserId", "FileId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitlePreferences",
                table: "SubtitlePreferences",
                columns: new[] { "UserId", "FileId" });
        }
    }
}
