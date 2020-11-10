using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class AddMediaTypePropToSubtitlePreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaType",
                table: "SubtitlePreferences",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "SubtitlePreferences");
        }
    }
}
