using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class TrackedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;
            migrationBuilder.DropColumn(
                name: "Created",
                table: "SubtitleFiles");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "FilmFiles");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "EpisodeFiles");
            
            
            migrationBuilder.RenameColumn(
                name: "Added",
                newName: "Created",
                table: "SubtitleFiles");

            migrationBuilder.RenameColumn(
                name: "Added",
                newName: "Created",
                table: "SeriesDetails");

            migrationBuilder.RenameColumn(
                name: "Added",
                newName: "Created",
                table: "FilmFiles");

            migrationBuilder.RenameColumn(
                name: "Added",
                newName: "Created",
                table: "FilmDetails");

            migrationBuilder.RenameColumn(
                name: "Added",
                newName: "Created",
                table: "EpisodeFiles");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Users",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Users",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "SubtitleFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "SubtitleFiles",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "SubtitleFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "SeriesDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "SeriesDetails",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "SeriesDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "SeasonDetails",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "SeasonDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "SeasonDetails",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "SeasonDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ProductionCompany",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "ProductionCompany",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ProductionCompany",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "ProductionCompany",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Network",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "Network",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Network",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "Network",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "MovieCollection",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "MovieCollection",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "MovieCollection",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "MovieCollection",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Libraries",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "Libraries",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Libraries",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "Libraries",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Genre",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "Genre",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Genre",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "Genre",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "FilmFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "FilmFiles",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "FilmFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "FilmDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "FilmDetails",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "FilmDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "EpisodeFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "EpisodeFiles",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "EpisodeFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "EpisodeDetails",
                nullable: false,
                defaultValue: now);
            
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "EpisodeDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "EpisodeDetails",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "EpisodeDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Creator",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCorrelationId",
                table: "Creator",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Creator",
                nullable: false,
                defaultValue: now);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedCorrelationId",
                table: "Creator",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WatchingProgresses_MediaType",
                table: "WatchingProgresses",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePreferences_MediaType",
                table: "SubtitlePreferences",
                column: "MediaType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;
            migrationBuilder.DropIndex(
                name: "IX_WatchingProgresses_MediaType",
                table: "WatchingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_SubtitlePreferences_MediaType",
                table: "SubtitlePreferences");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "SubtitleFiles");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "SubtitleFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "SubtitleFiles");

            migrationBuilder.RenameColumn(
                name: "Created",
                newName: "Added",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "SeriesDetails");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "SeasonDetails");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "SeasonDetails");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "SeasonDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "SeasonDetails");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ProductionCompany");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "ProductionCompany");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "ProductionCompany");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "ProductionCompany");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Network");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "Network");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Network");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "Network");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "MovieCollection");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Libraries");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "Libraries");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Libraries");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "Libraries");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "FilmFiles");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "FilmFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "FilmFiles");

            migrationBuilder.RenameColumn(
                name: "Created",
                newName: "Added",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "FilmDetails");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "EpisodeFiles");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "EpisodeFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "EpisodeFiles");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "EpisodeDetails");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "EpisodeDetails");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "EpisodeDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "EpisodeDetails");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "CreatedCorrelationId",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "UpdatedCorrelationId",
                table: "Creator");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "SubtitleFiles",
                nullable: false,
                defaultValue: now);
            
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "FilmFiles",
                nullable: false,
                defaultValue: now);
            
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "EpisodeFiles",
                nullable: false,
                defaultValue: now);
        }
    }
}
