using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations
{
    public partial class RenameTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbFilmDetails_MovieCollection_BelongsInCollectionId",
                table: "DbFilmDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeFiles_DbSeriesDetails_SeriesDetailsId",
                table: "EpisodeFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FilmFiles_DbFilmDetails_FilmDetailsId",
                table: "FilmFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_DbFilmDetail~",
                table: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_Entity1Id",
                table: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails_~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails~1",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFilm~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFil~1",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDet~",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDe~1",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_DbSeri~",
                table: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_Entity~",
                table: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_DbSeri~",
                table: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_Entity~",
                table: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDeta~",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDet~1",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDetails_DbSeriesDetails_DbSeriesDetailsId",
                table: "SeasonDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSeriesDetails",
                table: "DbSeriesDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbFilmDetails",
                table: "DbFilmDetails");

            migrationBuilder.RenameTable(
                name: "DbSeriesDetails",
                newName: "SeriesDetails");

            migrationBuilder.RenameTable(
                name: "DbFilmDetails",
                newName: "FilmDetails");

            migrationBuilder.RenameIndex(
                name: "IX_DbFilmDetails_BelongsInCollectionId",
                table: "FilmDetails",
                newName: "IX_FilmDetails_BelongsInCollectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeriesDetails",
                table: "SeriesDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilmDetails",
                table: "FilmDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeFiles_SeriesDetails_SeriesDetailsId",
                table: "EpisodeFiles",
                column: "SeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FilmDetails_MovieCollection_BelongsInCollectionId",
                table: "FilmDetails",
                column: "BelongsInCollectionId",
                principalTable: "MovieCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmDetailsId",
                table: "FilmFiles",
                column: "FilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_DbFilmDetailsId",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "DbFilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_Entity1Id",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "Entity1Id",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_Db~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "DbFilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_En~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "Entity1Id",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmDe~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "DbFilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmD~1",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "Entity1Id",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDetai~",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "DbFilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDeta~1",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "Entity1Id",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "DbSeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "Entity1Id",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_DbSeriesDe~",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "DbSeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "Entity1Id",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "DbSeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "Entity1Id",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetail~",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "DbSeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetai~1",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "Entity1Id",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDetails_SeriesDetails_DbSeriesDetailsId",
                table: "SeasonDetails",
                column: "DbSeriesDetailsId",
                principalTable: "SeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeFiles_SeriesDetails_SeriesDetailsId",
                table: "EpisodeFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FilmDetails_MovieCollection_BelongsInCollectionId",
                table: "FilmDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmDetailsId",
                table: "FilmFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_DbFilmDetailsId",
                table: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_Entity1Id",
                table: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_Db~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_En~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmDe~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmD~1",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDetai~",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDeta~1",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_DbSeriesDe~",
                table: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetail~",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetai~1",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDetails_SeriesDetails_DbSeriesDetailsId",
                table: "SeasonDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeriesDetails",
                table: "SeriesDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FilmDetails",
                table: "FilmDetails");

            migrationBuilder.RenameTable(
                name: "SeriesDetails",
                newName: "DbSeriesDetails");

            migrationBuilder.RenameTable(
                name: "FilmDetails",
                newName: "DbFilmDetails");

            migrationBuilder.RenameIndex(
                name: "IX_FilmDetails_BelongsInCollectionId",
                table: "DbFilmDetails",
                newName: "IX_DbFilmDetails_BelongsInCollectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSeriesDetails",
                table: "DbSeriesDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbFilmDetails",
                table: "DbFilmDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DbFilmDetails_MovieCollection_BelongsInCollectionId",
                table: "DbFilmDetails",
                column: "BelongsInCollectionId",
                principalTable: "MovieCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeFiles_DbSeriesDetails_SeriesDetailsId",
                table: "EpisodeFiles",
                column: "SeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FilmFiles_DbFilmDetails_FilmDetailsId",
                table: "FilmFiles",
                column: "FilmDetailsId",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_DbFilmDetail~",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "DbFilmDetailsId",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_Entity1Id",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "Entity1Id",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails_~",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "DbFilmDetailsId",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails~1",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "Entity1Id",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFilm~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "DbFilmDetailsId",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFil~1",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "Entity1Id",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDet~",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "DbFilmDetailsId",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDe~1",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "Entity1Id",
                principalTable: "DbFilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_DbSeri~",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "DbSeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_Entity~",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "Entity1Id",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_DbSeries~",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "DbSeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_Entity1Id",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "Entity1Id",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_DbSeri~",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "DbSeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_Entity~",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "Entity1Id",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDeta~",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "DbSeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDet~1",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "Entity1Id",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDetails_DbSeriesDetails_DbSeriesDetailsId",
                table: "SeasonDetails",
                column: "DbSeriesDetailsId",
                principalTable: "DbSeriesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
