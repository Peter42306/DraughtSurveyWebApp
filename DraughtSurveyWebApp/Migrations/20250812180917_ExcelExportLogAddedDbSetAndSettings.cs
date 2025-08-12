using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ExcelExportLogAddedDbSetAndSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExcelExportLogs_TemplateId",
                table: "ExcelExportLogs",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelExportLogs_AspNetUsers_UserId",
                table: "ExcelExportLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelExportLogs_ExcelTemplates_TemplateId",
                table: "ExcelExportLogs",
                column: "TemplateId",
                principalTable: "ExcelTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcelExportLogs_AspNetUsers_UserId",
                table: "ExcelExportLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcelExportLogs_ExcelTemplates_TemplateId",
                table: "ExcelExportLogs");

            migrationBuilder.DropIndex(
                name: "IX_ExcelExportLogs_TemplateId",
                table: "ExcelExportLogs");
        }
    }
}
