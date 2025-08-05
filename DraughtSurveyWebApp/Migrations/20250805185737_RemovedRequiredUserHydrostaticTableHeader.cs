using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRequiredUserHydrostaticTableHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Swell",
                table: "DraughtsInputs",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Swell",
                table: "DraughtsInputs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);
        }
    }
}
