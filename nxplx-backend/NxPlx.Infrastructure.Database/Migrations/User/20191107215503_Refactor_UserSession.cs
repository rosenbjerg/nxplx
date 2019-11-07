using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Services.Database.Migrations.User
{
    public partial class Refactor_UserSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LibraryAccess",
                table: "UserSessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LibraryAccess",
                table: "UserSessions",
                type: "text",
                nullable: true);
        }
    }
}
