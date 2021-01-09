using Microsoft.EntityFrameworkCore.Migrations;

namespace NxPlx.Infrastructure.Database.Migrations
{
    public partial class AddMissingIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmId",
                table: "FilmFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSeriesDetailsProductionCompany",
                table: "DbSeriesDetailsProductionCompany");

            migrationBuilder.DropIndex(
                name: "IX_DbSeriesDetailsProductionCompany_ProductionCompaniesId",
                table: "DbSeriesDetailsProductionCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSeriesDetailsNetwork",
                table: "DbSeriesDetailsNetwork");

            migrationBuilder.DropIndex(
                name: "IX_DbSeriesDetailsNetwork_NetworksId",
                table: "DbSeriesDetailsNetwork");

            migrationBuilder.RenameColumn(
                name: "FilmId",
                table: "FilmFiles",
                newName: "FilmDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_FilmFiles_FilmId",
                table: "FilmFiles",
                newName: "IX_FilmFiles_FilmDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSeriesDetailsProductionCompany",
                table: "DbSeriesDetailsProductionCompany",
                columns: new[] { "ProductionCompaniesId", "SeriesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSeriesDetailsNetwork",
                table: "DbSeriesDetailsNetwork",
                columns: new[] { "NetworksId", "SeriesId" });

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsProductionCompany_SeriesId",
                table: "DbSeriesDetailsProductionCompany",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsNetwork_SeriesId",
                table: "DbSeriesDetailsNetwork",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmDetailsId",
                table: "FilmFiles",
                column: "FilmDetailsId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmDetailsId",
                table: "FilmFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSeriesDetailsProductionCompany",
                table: "DbSeriesDetailsProductionCompany");

            migrationBuilder.DropIndex(
                name: "IX_DbSeriesDetailsProductionCompany_SeriesId",
                table: "DbSeriesDetailsProductionCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSeriesDetailsNetwork",
                table: "DbSeriesDetailsNetwork");

            migrationBuilder.DropIndex(
                name: "IX_DbSeriesDetailsNetwork_SeriesId",
                table: "DbSeriesDetailsNetwork");

            migrationBuilder.RenameColumn(
                name: "FilmDetailsId",
                table: "FilmFiles",
                newName: "FilmId");

            migrationBuilder.RenameIndex(
                name: "IX_FilmFiles_FilmDetailsId",
                table: "FilmFiles",
                newName: "IX_FilmFiles_FilmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSeriesDetailsProductionCompany",
                table: "DbSeriesDetailsProductionCompany",
                columns: new[] { "SeriesId", "ProductionCompaniesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSeriesDetailsNetwork",
                table: "DbSeriesDetailsNetwork",
                columns: new[] { "SeriesId", "NetworksId" });

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsProductionCompany_ProductionCompaniesId",
                table: "DbSeriesDetailsProductionCompany",
                column: "ProductionCompaniesId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSeriesDetailsNetwork_NetworksId",
                table: "DbSeriesDetailsNetwork",
                column: "NetworksId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilmFiles_FilmDetails_FilmId",
                table: "FilmFiles",
                column: "FilmId",
                principalTable: "FilmDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
