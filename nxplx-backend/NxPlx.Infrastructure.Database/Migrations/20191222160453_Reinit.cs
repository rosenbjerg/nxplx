using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NxPlx.Services.Database.Migrations
{
    public partial class Reinit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Creator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DbSeriesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstAirDate = table.Column<DateTime>(nullable: true),
                    InProduction = table.Column<bool>(nullable: false),
                    LastAirDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OriginalName = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    BackdropPath = table.Column<string>(nullable: true),
                    OriginalLanguage = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    Popularity = table.Column<double>(nullable: false),
                    PosterPath = table.Column<string>(nullable: true),
                    VoteAverage = table.Column<double>(nullable: false),
                    VoteCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSeriesDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    Kind = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieCollection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    PosterPath = table.Column<string>(nullable: true),
                    BackdropPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Network",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    LogoPath = table.Column<string>(nullable: true),
                    OriginCountry = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCompany",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogoPath = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OriginCountry = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCompany", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCountry",
                columns: table => new
                {
                    Iso3166_1 = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCountry", x => x.Iso3166_1);
                });

            migrationBuilder.CreateTable(
                name: "SpokenLanguage",
                columns: table => new
                {
                    Iso639_1 = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpokenLanguage", x => x.Iso639_1);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    HasChangedPassword = table.Column<bool>(nullable: false),
                    Admin = table.Column<bool>(nullable: false),
                    LibraryAccessIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Creator>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbSeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Creator>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_DbSeri~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_DbSeriesDetails_Entity~",
                        column: x => x.Entity1Id,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Creator>_Creator_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Creator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AirDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    PosterPath = table.Column<string>(nullable: true),
                    SeasonNumber = table.Column<int>(nullable: false),
                    DbSeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonDetails_DbSeriesDetails_DbSeriesDetailsId",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Genre>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbSeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Genre>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_DbSeries~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_DbSeriesDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Genre>_Genre_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileSizeBytes = table.Column<long>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastWrite = table.Column<DateTime>(nullable: false),
                    MediaDetailsId = table.Column<int>(nullable: true),
                    MediaDetails_Id = table.Column<int>(nullable: true),
                    MediaDetails_VideoCodec = table.Column<string>(nullable: true),
                    MediaDetails_VideoFrameRate = table.Column<float>(nullable: true),
                    MediaDetails_VideoBitrate = table.Column<int>(nullable: true),
                    MediaDetails_VideoBitDepth = table.Column<int>(nullable: true),
                    MediaDetails_VideoHeight = table.Column<int>(nullable: true),
                    MediaDetails_VideoWidth = table.Column<int>(nullable: true),
                    MediaDetails_AudioCodec = table.Column<string>(nullable: true),
                    MediaDetails_AudioBitrate = table.Column<int>(nullable: true),
                    MediaDetails_AudioChannelLayout = table.Column<string>(nullable: true),
                    MediaDetails_AudioStreamIndex = table.Column<int>(nullable: true),
                    MediaDetails_Duration = table.Column<float>(nullable: true),
                    MediaDetails_VideoAspectRatio = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SeasonNumber = table.Column<int>(nullable: false),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    PartOfLibraryId = table.Column<int>(nullable: false),
                    SeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeFiles_Libraries_PartOfLibraryId",
                        column: x => x.PartOfLibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpisodeFiles_DbSeriesDetails_SeriesDetailsId",
                        column: x => x.SeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbFilmDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Adult = table.Column<bool>(nullable: false),
                    Budget = table.Column<long>(nullable: false),
                    ImdbId = table.Column<string>(nullable: true),
                    BelongsInCollectionId = table.Column<int>(nullable: true),
                    OriginalTitle = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: true),
                    Revenue = table.Column<long>(nullable: false),
                    Runtime = table.Column<int>(nullable: true),
                    Tagline = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    BackdropPath = table.Column<string>(nullable: true),
                    OriginalLanguage = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    Popularity = table.Column<float>(nullable: false),
                    PosterPath = table.Column<string>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: false),
                    VoteCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbFilmDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbFilmDetails_MovieCollection_BelongsInCollectionId",
                        column: x => x.BelongsInCollectionId,
                        principalTable: "MovieCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, Network>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbSeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, Network>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_DbSeri~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_DbSeriesDetails_Entity~",
                        column: x => x.Entity1Id,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, Network>_Network_Entity2Id",
                        column: x => x.Entity2Id,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbSeriesDetails, ProductionCompany>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbSeriesDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbSeriesDetails, ProductionCompany>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDeta~",
                        column: x => x.DbSeriesDetailsId,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_DbSeriesDet~1",
                        column: x => x.Entity1Id,
                        principalTable: "DbSeriesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbSeriesDetails, ProductionCompany>_ProductionCo~",
                        column: x => x.Entity2Id,
                        principalTable: "ProductionCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AirDate = table.Column<DateTime>(nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    ProductionCode = table.Column<string>(nullable: true),
                    SeasonNumber = table.Column<int>(nullable: false),
                    StillPath = table.Column<string>(nullable: true),
                    VoteAverage = table.Column<float>(nullable: false),
                    VoteCount = table.Column<int>(nullable: false),
                    SeasonDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeDetails_SeasonDetails_SeasonDetailsId",
                        column: x => x.SeasonDetailsId,
                        principalTable: "SeasonDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilmFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileSizeBytes = table.Column<long>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastWrite = table.Column<DateTime>(nullable: false),
                    MediaDetailsId = table.Column<int>(nullable: true),
                    MediaDetails_Id = table.Column<int>(nullable: true),
                    MediaDetails_VideoCodec = table.Column<string>(nullable: true),
                    MediaDetails_VideoFrameRate = table.Column<float>(nullable: true),
                    MediaDetails_VideoBitrate = table.Column<int>(nullable: true),
                    MediaDetails_VideoBitDepth = table.Column<int>(nullable: true),
                    MediaDetails_VideoHeight = table.Column<int>(nullable: true),
                    MediaDetails_VideoWidth = table.Column<int>(nullable: true),
                    MediaDetails_AudioCodec = table.Column<string>(nullable: true),
                    MediaDetails_AudioBitrate = table.Column<int>(nullable: true),
                    MediaDetails_AudioChannelLayout = table.Column<string>(nullable: true),
                    MediaDetails_AudioStreamIndex = table.Column<int>(nullable: true),
                    MediaDetails_Duration = table.Column<float>(nullable: true),
                    MediaDetails_VideoAspectRatio = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    FilmDetailsId = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    PartOfLibraryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmFiles_DbFilmDetails_FilmDetailsId",
                        column: x => x.FilmDetailsId,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FilmFiles_Libraries_PartOfLibraryId",
                        column: x => x.PartOfLibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinEntity<DbFilmDetails, Genre>",
                columns: table => new
                {
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbFilmDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, Genre>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_DbFilmDetail~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, Genre>_DbFilmDetails_Entity1Id",
                        column: x => x.Entity1Id,
                        principalTable: "DbFilmDetails",
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
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<int>(nullable: false),
                    DbFilmDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, ProductionCompany>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails_~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCompany>_DbFilmDetails~1",
                        column: x => x.Entity1Id,
                        principalTable: "DbFilmDetails",
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
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<string>(nullable: false),
                    DbFilmDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, ProductionCountry, string>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFilm~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, ProductionCountry, string>_DbFil~1",
                        column: x => x.Entity1Id,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Entity1Id = table.Column<int>(nullable: false),
                    Entity2Id = table.Column<string>(nullable: false),
                    DbFilmDetailsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinEntity<DbFilmDetails, SpokenLanguage, string>", x => new { x.Entity1Id, x.Entity2Id });
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDet~",
                        column: x => x.DbFilmDetailsId,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_DbFilmDe~1",
                        column: x => x.Entity1Id,
                        principalTable: "DbFilmDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinEntity<DbFilmDetails, SpokenLanguage, string>_SpokenLan~",
                        column: x => x.Entity2Id,
                        principalTable: "SpokenLanguage",
                        principalColumn: "Iso639_1",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitleFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileSizeBytes = table.Column<long>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastWrite = table.Column<DateTime>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    EpisodeFileId = table.Column<int>(nullable: true),
                    FilmFileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitleFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubtitleFiles_EpisodeFiles_EpisodeFileId",
                        column: x => x.EpisodeFileId,
                        principalTable: "EpisodeFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubtitleFiles_FilmFiles_FilmFileId",
                        column: x => x.FilmFileId,
                        principalTable: "FilmFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitlePreferences",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitlePreferences", x => new { x.UserId, x.FileId });
                    table.ForeignKey(
                        name: "FK_SubtitlePreferences_EpisodeFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "EpisodeFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubtitlePreferences_FilmFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "FilmFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubtitlePreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WatchingProgresses",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    LastWatched = table.Column<DateTime>(nullable: false),
                    Time = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchingProgresses", x => new { x.UserId, x.FileId });
                    table.ForeignKey(
                        name: "FK_WatchingProgresses_EpisodeFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "EpisodeFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchingProgresses_FilmFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "FilmFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchingProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbFilmDetails_BelongsInCollectionId",
                table: "DbFilmDetails",
                column: "BelongsInCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeDetails_SeasonDetailsId",
                table: "EpisodeDetails",
                column: "SeasonDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeFiles_PartOfLibraryId",
                table: "EpisodeFiles",
                column: "PartOfLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeFiles_SeasonNumber",
                table: "EpisodeFiles",
                column: "SeasonNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeFiles_SeriesDetailsId",
                table: "EpisodeFiles",
                column: "SeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmFiles_FilmDetailsId",
                table: "FilmFiles",
                column: "FilmDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmFiles_PartOfLibraryId",
                table: "FilmFiles",
                column: "PartOfLibraryId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDetails_DbSeriesDetailsId",
                table: "SeasonDetails",
                column: "DbSeriesDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleFiles_EpisodeFileId",
                table: "SubtitleFiles",
                column: "EpisodeFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleFiles_FilmFileId",
                table: "SubtitleFiles",
                column: "FilmFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePreferences_FileId",
                table: "SubtitlePreferences",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchingProgresses_FileId",
                table: "WatchingProgresses",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpisodeDetails");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbFilmDetails, Genre>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbFilmDetails, ProductionCompany>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbFilmDetails, ProductionCountry, string>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbFilmDetails, SpokenLanguage, string>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbSeriesDetails, Creator>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbSeriesDetails, Genre>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbSeriesDetails, Network>");

            migrationBuilder.DropTable(
                name: "JoinEntity<DbSeriesDetails, ProductionCompany>");

            migrationBuilder.DropTable(
                name: "SubtitleFiles");

            migrationBuilder.DropTable(
                name: "SubtitlePreferences");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WatchingProgresses");

            migrationBuilder.DropTable(
                name: "SeasonDetails");

            migrationBuilder.DropTable(
                name: "ProductionCountry");

            migrationBuilder.DropTable(
                name: "SpokenLanguage");

            migrationBuilder.DropTable(
                name: "Creator");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Network");

            migrationBuilder.DropTable(
                name: "ProductionCompany");

            migrationBuilder.DropTable(
                name: "EpisodeFiles");

            migrationBuilder.DropTable(
                name: "FilmFiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DbSeriesDetails");

            migrationBuilder.DropTable(
                name: "DbFilmDetails");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.DropTable(
                name: "MovieCollection");
        }
    }
}
