﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NxPlx.Services.Database;

namespace NxPlx.Services.Database.Migrations.Media
{
    [DbContext(typeof(MediaContext))]
    partial class MediaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("NxPlx.Models.Database.DbFilmDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Adult")
                        .HasColumnType("boolean");

                    b.Property<string>("BackdropPath")
                        .HasColumnType("text");

                    b.Property<int?>("BelongsInCollectionId")
                        .HasColumnType("integer");

                    b.Property<long>("Budget")
                        .HasColumnType("bigint");

                    b.Property<string>("ImdbId")
                        .HasColumnType("text");

                    b.Property<string>("OriginalLanguage")
                        .HasColumnType("text");

                    b.Property<string>("OriginalTitle")
                        .HasColumnType("text");

                    b.Property<string>("Overview")
                        .HasColumnType("text");

                    b.Property<float>("Popularity")
                        .HasColumnType("real");

                    b.Property<string>("PosterPath")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("Revenue")
                        .HasColumnType("bigint");

                    b.Property<int?>("Runtime")
                        .HasColumnType("integer");

                    b.Property<string>("Tagline")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<float>("VoteAverage")
                        .HasColumnType("real");

                    b.Property<int>("VoteCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BelongsInCollectionId");

                    b.ToTable("DbFilmDetails");
                });

            modelBuilder.Entity("NxPlx.Models.Database.DbSeriesDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("BackdropPath")
                        .HasColumnType("text");

                    b.Property<DateTime?>("FirstAirDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("InProduction")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastAirDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OriginalLanguage")
                        .HasColumnType("text");

                    b.Property<string>("OriginalName")
                        .HasColumnType("text");

                    b.Property<string>("Overview")
                        .HasColumnType("text");

                    b.Property<double>("Popularity")
                        .HasColumnType("double precision");

                    b.Property<string>("PosterPath")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.Property<double>("VoteAverage")
                        .HasColumnType("double precision");

                    b.Property<int>("VoteCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DbSeriesDetails");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Film.ProductionCountry, string>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<string>("Entity2Id")
                        .HasColumnType("text");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbFilmDetails, ProductionCountry, string>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Film.SpokenLanguage, string>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<string>("Entity2Id")
                        .HasColumnType("text");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbFilmDetails, SpokenLanguage, string>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Genre>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbFilmDetails, Genre>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.ProductionCompany>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbFilmDetails, ProductionCompany>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Genre>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbSeriesDetails, Genre>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.ProductionCompany>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbSeriesDetails, ProductionCompany>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Series.Creator>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbSeriesDetails, Creator>");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Series.Network>", b =>
                {
                    b.Property<int>("Entity1Id")
                        .HasColumnType("integer");

                    b.Property<int>("Entity2Id")
                        .HasColumnType("integer");

                    b.HasKey("Entity1Id", "Entity2Id");

                    b.HasIndex("Entity2Id");

                    b.ToTable("JoinEntity<DbSeriesDetails, Network>");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Film.MovieCollection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("BackdropPath")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PosterPath")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("MovieCollection");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Film.ProductionCountry", b =>
                {
                    b.Property<string>("Iso3166_1")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Iso3166_1");

                    b.ToTable("ProductionCountry");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Film.SpokenLanguage", b =>
                {
                    b.Property<string>("Iso639_1")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Iso639_1");

                    b.ToTable("SpokenLanguage");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Genre");
                });

            modelBuilder.Entity("NxPlx.Models.Details.ProductionCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("LogoPath")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OriginCountry")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductionCompany");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.Creator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Creator");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.EpisodeDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("AirDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("EpisodeNumber")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Overview")
                        .HasColumnType("text");

                    b.Property<string>("ProductionCode")
                        .HasColumnType("text");

                    b.Property<int?>("SeasonDetailsId")
                        .HasColumnType("integer");

                    b.Property<int>("SeasonNumber")
                        .HasColumnType("integer");

                    b.Property<string>("StillPath")
                        .HasColumnType("text");

                    b.Property<float>("VoteAverage")
                        .HasColumnType("real");

                    b.Property<int>("VoteCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SeasonDetailsId");

                    b.ToTable("EpisodeDetails");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.Network", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("LogoPath")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OriginCountry")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Network");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.SeasonDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("AirDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DbSeriesDetailsId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Overview")
                        .HasColumnType("text");

                    b.Property<string>("PosterPath")
                        .HasColumnType("text");

                    b.Property<int>("SeasonNumber")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DbSeriesDetailsId");

                    b.ToTable("SeasonDetails");
                });

            modelBuilder.Entity("NxPlx.Models.File.EpisodeFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("EpisodeNumber")
                        .HasColumnType("integer");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastWrite")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("MediaDetailsId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PartOfLibraryId")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<int>("SeasonNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("SeriesDetailsId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MediaDetailsId");

                    b.HasIndex("PartOfLibraryId");

                    b.HasIndex("SeasonNumber");

                    b.HasIndex("SeriesDetailsId");

                    b.ToTable("EpisodeFiles");
                });

            modelBuilder.Entity("NxPlx.Models.File.FFMpegProbeDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AudioBitrate")
                        .HasColumnType("integer");

                    b.Property<string>("AudioChannelLayout")
                        .HasColumnType("text");

                    b.Property<string>("AudioCodec")
                        .HasColumnType("text");

                    b.Property<string>("AudioCodecName")
                        .HasColumnType("text");

                    b.Property<int>("AudioStreamIndex")
                        .HasColumnType("integer");

                    b.Property<float>("Duration")
                        .HasColumnType("real");

                    b.Property<string>("VideoAspectRatio")
                        .HasColumnType("text");

                    b.Property<int>("VideoBitDepth")
                        .HasColumnType("integer");

                    b.Property<int>("VideoBitrate")
                        .HasColumnType("integer");

                    b.Property<string>("VideoCodec")
                        .HasColumnType("text");

                    b.Property<string>("VideoCodecName")
                        .HasColumnType("text");

                    b.Property<float>("VideoFrameRate")
                        .HasColumnType("real");

                    b.Property<int>("VideoHeight")
                        .HasColumnType("integer");

                    b.Property<int>("VideoWidth")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("FFMpegProbeDetails");
                });

            modelBuilder.Entity("NxPlx.Models.File.FilmFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<int?>("FilmDetailsId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastWrite")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("MediaDetailsId")
                        .HasColumnType("integer");

                    b.Property<int>("PartOfLibraryId")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FilmDetailsId");

                    b.HasIndex("MediaDetailsId");

                    b.HasIndex("PartOfLibraryId");

                    b.ToTable("FilmFiles");
                });

            modelBuilder.Entity("NxPlx.Models.File.SubtitleFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("EpisodeFileId")
                        .HasColumnType("integer");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<int?>("FilmFileId")
                        .HasColumnType("integer");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastWrite")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeFileId");

                    b.HasIndex("FilmFileId");

                    b.ToTable("SubtitleFiles");
                });

            modelBuilder.Entity("NxPlx.Models.Library", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Kind")
                        .HasColumnType("integer");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Libraries");
                });

            modelBuilder.Entity("NxPlx.Models.Database.DbFilmDetails", b =>
                {
                    b.HasOne("NxPlx.Models.Details.Film.MovieCollection", "BelongsInCollection")
                        .WithMany()
                        .HasForeignKey("BelongsInCollectionId");
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Film.ProductionCountry, string>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbFilmDetails", "Entity1")
                        .WithMany("ProductionCountries")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Film.ProductionCountry", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Film.SpokenLanguage, string>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbFilmDetails", "Entity1")
                        .WithMany("SpokenLanguages")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Film.SpokenLanguage", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.Genre>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbFilmDetails", "Entity1")
                        .WithMany("Genres")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Genre", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbFilmDetails, NxPlx.Models.Details.ProductionCompany>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbFilmDetails", "Entity1")
                        .WithMany("ProductionCompanies")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.ProductionCompany", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Genre>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", "Entity1")
                        .WithMany("Genres")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Genre", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.ProductionCompany>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", "Entity1")
                        .WithMany("ProductionCompanies")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.ProductionCompany", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Series.Creator>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", "Entity1")
                        .WithMany("CreatedBy")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Series.Creator", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Database.JoinEntity<NxPlx.Models.Database.DbSeriesDetails, NxPlx.Models.Details.Series.Network>", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", "Entity1")
                        .WithMany("Networks")
                        .HasForeignKey("Entity1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Details.Series.Network", "Entity2")
                        .WithMany()
                        .HasForeignKey("Entity2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.EpisodeDetails", b =>
                {
                    b.HasOne("NxPlx.Models.Details.Series.SeasonDetails", null)
                        .WithMany("Episodes")
                        .HasForeignKey("SeasonDetailsId");
                });

            modelBuilder.Entity("NxPlx.Models.Details.Series.SeasonDetails", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", null)
                        .WithMany("Seasons")
                        .HasForeignKey("DbSeriesDetailsId");
                });

            modelBuilder.Entity("NxPlx.Models.File.EpisodeFile", b =>
                {
                    b.HasOne("NxPlx.Models.File.FFMpegProbeDetails", "MediaDetails")
                        .WithMany()
                        .HasForeignKey("MediaDetailsId");

                    b.HasOne("NxPlx.Models.Library", "PartOfLibrary")
                        .WithMany()
                        .HasForeignKey("PartOfLibraryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NxPlx.Models.Database.DbSeriesDetails", "SeriesDetails")
                        .WithMany()
                        .HasForeignKey("SeriesDetailsId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("NxPlx.Models.File.FilmFile", b =>
                {
                    b.HasOne("NxPlx.Models.Database.DbFilmDetails", "FilmDetails")
                        .WithMany()
                        .HasForeignKey("FilmDetailsId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("NxPlx.Models.File.FFMpegProbeDetails", "MediaDetails")
                        .WithMany()
                        .HasForeignKey("MediaDetailsId");

                    b.HasOne("NxPlx.Models.Library", "PartOfLibrary")
                        .WithMany()
                        .HasForeignKey("PartOfLibraryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NxPlx.Models.File.SubtitleFile", b =>
                {
                    b.HasOne("NxPlx.Models.File.EpisodeFile", null)
                        .WithMany("Subtitles")
                        .HasForeignKey("EpisodeFileId");

                    b.HasOne("NxPlx.Models.File.FilmFile", null)
                        .WithMany("Subtitles")
                        .HasForeignKey("FilmFileId");
                });
#pragma warning restore 612, 618
        }
    }
}
