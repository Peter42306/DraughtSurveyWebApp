using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ExcelExportLogAddedDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelExportLog",
                table: "ExcelExportLog");

            migrationBuilder.RenameTable(
                name: "ExcelExportLog",
                newName: "ExcelExportLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelExportLog_UserId",
                table: "ExcelExportLogs",
                newName: "IX_ExcelExportLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelExportLog_CreatedUtc",
                table: "ExcelExportLogs",
                newName: "IX_ExcelExportLogs_CreatedUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelExportLogs",
                table: "ExcelExportLogs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelExportLogs",
                table: "ExcelExportLogs");

            migrationBuilder.RenameTable(
                name: "ExcelExportLogs",
                newName: "ExcelExportLog");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelExportLogs_UserId",
                table: "ExcelExportLog",
                newName: "IX_ExcelExportLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelExportLogs_CreatedUtc",
                table: "ExcelExportLog",
                newName: "IX_ExcelExportLog_CreatedUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelExportLog",
                table: "ExcelExportLog",
                column: "Id");
        }
    }
}
