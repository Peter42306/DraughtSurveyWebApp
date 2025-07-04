using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDeductiblesResultsToDraughtSurveyBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeductiblesResults_DraughtSurveyBlockId",
                table: "DeductiblesResults");

            migrationBuilder.CreateIndex(
                name: "IX_DeductiblesResults_DraughtSurveyBlockId",
                table: "DeductiblesResults",
                column: "DraughtSurveyBlockId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeductiblesResults_DraughtSurveyBlockId",
                table: "DeductiblesResults");

            migrationBuilder.CreateIndex(
                name: "IX_DeductiblesResults_DraughtSurveyBlockId",
                table: "DeductiblesResults",
                column: "DraughtSurveyBlockId");
        }
    }
}
