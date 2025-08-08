using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSetExcelTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcelTemplate_AspNetUsers_OwnerId",
                table: "ExcelTemplate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelTemplate",
                table: "ExcelTemplate");

            migrationBuilder.RenameTable(
                name: "ExcelTemplate",
                newName: "ExcelTemplates");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelTemplate_OwnerId",
                table: "ExcelTemplates",
                newName: "IX_ExcelTemplates_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelTemplate_Name",
                table: "ExcelTemplates",
                newName: "IX_ExcelTemplates_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelTemplates",
                table: "ExcelTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelTemplates_AspNetUsers_OwnerId",
                table: "ExcelTemplates",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcelTemplates_AspNetUsers_OwnerId",
                table: "ExcelTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelTemplates",
                table: "ExcelTemplates");

            migrationBuilder.RenameTable(
                name: "ExcelTemplates",
                newName: "ExcelTemplate");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelTemplates_OwnerId",
                table: "ExcelTemplate",
                newName: "IX_ExcelTemplate_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelTemplates_Name",
                table: "ExcelTemplate",
                newName: "IX_ExcelTemplate_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelTemplate",
                table: "ExcelTemplate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelTemplate_AspNetUsers_OwnerId",
                table: "ExcelTemplate",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
