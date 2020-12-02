using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class UseImplicitJoinEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreatorDbSeriesDetails",
                columns: table => new
                {
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedSeriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorDbSeriesDetails", x => new { x.CreatedById, x.CreatedSeriesId });
                    table.ForeignKey(
                        name: "FK_CreatorDbSeriesDetails_Creator_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Creator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorDbSeriesDetails_SeriesDetails_CreatedSeriesId",
                        column: x => x.CreatedSeriesId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"CreatorDbSeriesDetails\" (\"CreatedById\", \"CreatedSeriesId\") SELECT \"Entity2Id\", \"Entity1Id\" FROM \"JoinEntity<DbSeriesDetails, Creator>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.CreateTable(
                name: "DbFilmDetailsGenre",
                columns: table => new
                {
                    FilmDetailsId = table.Column<int>(type: "integer", nullable: false),
                    GenresId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbFilmDetailsGenre", x => new { x.FilmDetailsId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsGenre_FilmDetails_FilmId",
                        column: x => x.FilmDetailsId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsGenre_Genre_GenreId",
                        column: x => x.GenresId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbFilmDetailsGenre\" (\"FilmDetailsId\", \"GenresId\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"JoinEntity<DbFilmDetails, Genre>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.CreateTable(
                name: "DbFilmDetailsProductionCompany",
                columns: table => new
                {
                    ProducedFilmId = table.Column<int>(type: "integer", nullable: false),
                    ProductionCompaniesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbFilmDetailsProductionCompany", x => new { x.ProducedFilmId, x.ProductionCompaniesId });
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsProductionCompany_FilmDetails_ProducedFilmId",
                        column: x => x.ProducedFilmId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsProductionCompany_ProductionCompany_Production~",
                        column: x => x.ProductionCompaniesId,
                        principalTable: "ProductionCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbFilmDetailsProductionCompany\" (\"ProducedFilmId\", \"ProductionCompaniesId\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"public\".\"JoinEntity<DbFilmDetails, ProductionCompany>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.CreateTable(
                name: "DbFilmDetailsProductionCountry",
                columns: table => new
                {
                    FilmedOnLocationId = table.Column<int>(type: "integer", nullable: false),
                    ProductionCountriesIso3166_1 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbFilmDetailsProductionCountry", x => new { x.FilmedOnLocationId, x.ProductionCountriesIso3166_1 });
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsProductionCountry_FilmDetails_FilmedOnLocation~",
                        column: x => x.FilmedOnLocationId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsProductionCountry_ProductionCountry_Production~",
                        column: x => x.ProductionCountriesIso3166_1,
                        principalTable: "ProductionCountry",
                        principalColumn: "Iso3166_1",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbFilmDetailsProductionCountry\" (\"FilmedOnLocationId\", \"ProductionCountriesIso3166_1\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"JoinEntity<DbFilmDetails, ProductionCountry, string>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.CreateTable(
                name: "DbFilmDetailsSpokenLanguage",
                columns: table => new
                {
                    SpokenInFilmId = table.Column<int>(type: "integer", nullable: false),
                    SpokenLanguagesIso639_1 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbFilmDetailsSpokenLanguage", x => new { x.SpokenInFilmId, x.SpokenLanguagesIso639_1 });
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsSpokenLanguage_FilmDetails_SpokenInFilmId",
                        column: x => x.SpokenInFilmId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbFilmDetailsSpokenLanguage_SpokenLanguage_SpokenLanguagesI~",
                        column: x => x.SpokenLanguagesIso639_1,
                        principalTable: "SpokenLanguage",
                        principalColumn: "Iso639_1",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbFilmDetailsSpokenLanguage\" (\"SpokenInFilmId\", \"SpokenLanguagesIso639_1\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"JoinEntity<DbFilmDetails, SpokenLanguage, string>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.CreateTable(
                name: "DbSeriesDetailsGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    SeriesInGenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSeriesDetailsGenre", x => new { x.GenresId, x.SeriesInGenreId });
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsGenre_Genre_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsGenre_SeriesDetails_SeriesInGenreId",
                        column: x => x.SeriesInGenreId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbSeriesDetailsGenre\" (\"GenresId\", \"SeriesInGenreId\") SELECT \"Entity2Id\", \"Entity1Id\" FROM \"JoinEntity<DbSeriesDetails, Genre>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.CreateTable(
                name: "DbSeriesDetailsNetwork",
                columns: table => new
                {
                    AiredSeriesId = table.Column<int>(type: "integer", nullable: false),
                    NetworksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSeriesDetailsNetwork", x => new { x.AiredSeriesId, x.NetworksId });
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsNetwork_Network_NetworksId",
                        column: x => x.NetworksId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsNetwork_SeriesDetails_AiredSeriesId",
                        column: x => x.AiredSeriesId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbSeriesDetailsNetwork\" (\"AiredSeriesId\", \"NetworksId\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"JoinEntity<DbSeriesDetails, Network>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.CreateTable(
                name: "DbSeriesDetailsProductionCompany",
                columns: table => new
                {
                    ProducedSeriesId = table.Column<int>(type: "integer", nullable: false),
                    ProductionCompaniesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSeriesDetailsProductionCompany", x => new { x.ProducedSeriesId, x.ProductionCompaniesId });
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsProductionCompany_ProductionCompany_Producti~",
                        column: x => x.ProductionCompaniesId,
                        principalTable: "ProductionCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbSeriesDetailsProductionCompany_SeriesDetails_ProducedSeri~",
                        column: x => x.ProducedSeriesId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql("INSERT INTO \"DbSeriesDetailsProductionCompany\" (\"ProducedSeriesId\", \"ProductionCompaniesId\") SELECT \"Entity1Id\", \"Entity2Id\" FROM \"JoinEntity<DbSeriesDetails, ProductionCompany>\"");
            migrationBuilder.DropTable(name: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorDbSeriesDetails_CreatedSeriesId",
                table: "CreatorDbSeriesDetails",
                column: "CreatedSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_DbFilmDetailsGenre_GenresId",
                table: "DbFilmDetailsGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_DbFilmDetailsProductionCompany_ProductionCompaniesId",
                table: "DbFilmDetailsProductionCompany",
                column: "ProductionCompaniesId");

            migrationBuilder.CreateIndex(
                name: "IX_DbFilmDetailsProductionCountry_ProductionCountriesIso3166_1",
                table: "DbFilmDetailsProductionCountry",
                column: "ProductionCountriesIso3166_1");

            migrationBuilder.CreateIndex(
                name: "IX_DbFilmDetailsSpokenLanguage_SpokenLanguagesIso639_1",
                table: "DbFilmDetailsSpokenLanguage",
                column: "SpokenLanguagesIso639_1");

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsGenre_SeriesInGenreId",
                table: "DbSeriesDetailsGenre",
                column: "SeriesInGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsNetwork_NetworksId",
                table: "DbSeriesDetailsNetwork",
                column: "NetworksId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsProductionCompany_ProductionCompaniesId",
                table: "DbSeriesDetailsProductionCompany",
                column: "ProductionCompaniesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoinEntity<DbFilmDetails, Genre>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbFilmDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, Genre>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_DbFilmDetailsId",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, Genre>_FilmDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, Genre>_Genre_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbFilmDetails, ProductionCompany>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbFilmDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, ProductionCompany>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_Db~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_FilmDetails_En~",
                        column: x => x.Entity1Id,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_ProductionComp~",
                        column: x => x.Entity2Id,
                        principalTable: "ProductionCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<string>(type: "text", nullable: false),
                    DbFilmDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, ProductionCountry, string>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmD~1",
                        column: x => x.Entity1Id,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_FilmDe~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_Produc~",
                        column: x => x.Entity2Id,
                        principalTable: "ProductionCountry",
                        principalColumn: "Iso3166_1",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<string>(type: "text", nullable: false),
                    DbFilmDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, SpokenLanguage, string>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDeta~1",
                        column: x => x.Entity1Id,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_FilmDetai~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "FilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_SpokenLan~",
                        column: x => x.Entity2Id,
                        principalTable: "SpokenLanguage",
                        principalColumn: "Iso639_1",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Creator>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbSeriesDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Creator>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_Creator_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Creator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_DbSeries~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_SeriesDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Genre>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbSeriesDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Genre>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_Genre_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_DbSeriesDe~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_SeriesDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Network>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbSeriesDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Network>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_Network_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_DbSeries~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_SeriesDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(type: "integer", nullable: false),
                    Entity2Id = table.Column<int>(type: "integer", nullable: false),
                    DbSeriesDetailsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, ProductionCompany>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_ProductionCo~",
                        column: x => x.Entity2Id,
                        principalTable: "ProductionCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetai~1",
                        column: x => x.Entity1Id,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_SeriesDetail~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "SeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbSeriesDetails, Creator>\" (\"Entity2Id\", \"Entity1Id\") SELECT \"CreatedById\", \"CreatedSeriesId\" FROM \"CreatorDbSeriesDetails\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbFilmDetails, Genre>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"FilmDetailsId\", \"GenresId\" FROM \"DbFilmDetailsGenre\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbFilmDetails, ProductionCompany>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"ProducedFilmId\", \"ProductionCompaniesId\" FROM \"DbFilmDetailsProductionCompany\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbFilmDetails, ProductionCountry, string>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"FilmedOnLocationId\", \"ProductionCountriesIso3166_1\" FROM \"DbFilmDetailsProductionCountry\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbFilmDetails, SpokenLanguage, string>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"SpokenInFilmId\", \"SpokenLanguagesIso639_1\" FROM \"DbFilmDetailsSpokenLanguage\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbSeriesDetails, Genre>\" (\"Entity2Id\", \"Entity1Id\") SELECT \"GenresId\", \"SeriesInGenreId\" FROM \"DbSeriesDetailsGenre\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbSeriesDetails, Network>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"AiredSeriesId\", \"NetworksId\" FROM \"DbSeriesDetailsNetwork\"");
            migrationBuilder.Sql("INSERT INTO \"JoinEntity<DbSeriesDetails, ProductionCompany>\" (\"Entity1Id\", \"Entity2Id\") SELECT \"ProducedSeriesId\", \"ProductionCompaniesId\" FROM \"DbSeriesDetailsProductionCompany\"");
            
            migrationBuilder.DropTable(
                name: "CreatorDbSeriesDetails");

            migrationBuilder.DropTable(
                name: "DbFilmDetailsGenre");

            migrationBuilder.DropTable(
                name: "DbFilmDetailsProductionCompany");

            migrationBuilder.DropTable(
                name: "DbFilmDetailsProductionCountry");

            migrationBuilder.DropTable(
                name: "DbFilmDetailsSpokenLanguage");

            migrationBuilder.DropTable(
                name: "DbSeriesDetailsGenre");

            migrationBuilder.DropTable(
                name: "DbSeriesDetailsNetwork");

            migrationBuilder.DropTable(
                name: "DbSeriesDetailsProductionCompany");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, Genre>_DbFilmDetailsId",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "DbFilmDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, Genre>_Entity2Id",
                table: "JoinEntity<DbFilmDetails, Genre>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetailsId",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "DbFilmDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, ProductionCompany>_Entity2Id",
                table: "JoinEntity<DbFilmDetails, ProductionCompany>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFilm~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "DbFilmDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, ProductionCountry, string>_Entity~",
                table: "JoinEntity<DbFilmDetails, ProductionCountry, string>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDet~",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "DbFilmDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbFilmDetails, SpokenLanguage, string>_Entity2Id",
                table: "JoinEntity<DbFilmDetails, SpokenLanguage, string>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetailsId",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "DbSeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Creator>_Entity2Id",
                table: "JoinEntity<DbSeriesDetails, Creator>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetailsId",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "DbSeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Genre>_Entity2Id",
                table: "JoinEntity<DbSeriesDetails, Genre>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetailsId",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "DbSeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, Network>_Entity2Id",
                table: "JoinEntity<DbSeriesDetails, Network>",
                column: "Entity2Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDeta~",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "DbSeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinEntity<DbSeriesDetails, ProductionCompany>_Entity2Id",
                table: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                column: "Entity2Id");
        }
    }
}
