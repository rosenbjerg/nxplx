using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class Refactor_MediaDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioChannels",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "AudioSamplingRateHz",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "WebOptimized",
                table: "FFMpegProbeDetails");

            migrationBuilder.AlterColumn<float>(
                name: "VideoFrameRate",
                table: "FFMpegProbeDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "AudioChannelLayout",
                table: "FFMpegProbeDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudioCodecName",
                table: "FFMpegProbeDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AudioStreamIndex",
                table: "FFMpegProbeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Duration",
                table: "FFMpegProbeDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "VideoAspectRatio",
                table: "FFMpegProbeDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoCodecName",
                table: "FFMpegProbeDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioChannelLayout",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "AudioCodecName",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "AudioStreamIndex",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "VideoAspectRatio",
                table: "FFMpegProbeDetails");

            migrationBuilder.DropColumn(
                name: "VideoCodecName",
                table: "FFMpegProbeDetails");

            migrationBuilder.AlterColumn<int>(
                name: "VideoFrameRate",
                table: "FFMpegProbeDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AddColumn<int>(
                name: "AudioChannels",
                table: "FFMpegProbeDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AudioSamplingRateHz",
                table: "FFMpegProbeDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WebOptimized",
                table: "FFMpegProbeDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
