using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class AddBlurHashColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaDetails_Id",
                table: "FilmFiles");

            migrationBuilder.DropColumn(
                name: "MediaDetails_Id",
                table: "EpisodeFiles");

            migrationBuilder.AddColumn<string>(
                name: "BackdropBlurHash",
                table: "SeriesDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterBlurHash",
                table: "SeriesDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterBlurHash",
                table: "SeasonDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoBlurHash",
                table: "ProductionCompany",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoBlurHash",
                table: "Network",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackdropBlurHash",
                table: "MovieCollection",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterBlurHash",
                table: "MovieCollection",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackdropBlurHash",
                table: "FilmDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterBlurHash",
                table: "FilmDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StillBlurHash",
                table: "EpisodeDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropBlurHash",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "PosterBlurHash",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "PosterBlurHash",
                table: "SeasonDetails");

            migrationBuilder.DropColumn(
                name: "LogoBlurHash",
                table: "ProductionCompany");

            migrationBuilder.DropColumn(
                name: "LogoBlurHash",
                table: "Network");

            migrationBuilder.DropColumn(
                name: "BackdropBlurHash",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "PosterBlurHash",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "BackdropBlurHash",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "PosterBlurHash",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "StillBlurHash",
                table: "EpisodeDetails");

            migrationBuilder.AddColumn<int>(
                name: "MediaDetails_Id",
                table: "FilmFiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaDetails_Id",
                table: "EpisodeFiles",
                type: "integer",
                nullable: true);
        }
    }
}
