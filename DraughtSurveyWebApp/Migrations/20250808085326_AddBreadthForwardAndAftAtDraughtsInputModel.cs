using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBreadthForwardAndAftAtDraughtsInputModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BreadthAft",
                table: "DraughtsInputs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BreadthForward",
                table: "DraughtsInputs",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreadthAft",
                table: "DraughtsInputs");

            migrationBuilder.DropColumn(
                name: "BreadthForward",
                table: "DraughtsInputs");
        }
    }
}
