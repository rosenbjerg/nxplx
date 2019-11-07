using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NxPlx.Services.Database.Migrations.User
{
    public partial class Init_User_DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    LibraryAccess = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubtitlePreferences");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WatchingProgresses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
