using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations.Media
{
    public partial class RemoveLongCodecName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioCodecName",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "VideoCodecName",
                table: "FFMpegProbeDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioCodecName",
                table: "FFMpegProbeDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoCodecName",
                table: "FFMpegProbeDetails",
                type: "text",
                nullable: true);
        }
    }
}
