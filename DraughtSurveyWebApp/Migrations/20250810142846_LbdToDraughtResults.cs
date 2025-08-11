using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class LbdToDraughtResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LBD",
                table: "DraughtsResults",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LBD",
                table: "DraughtsResults");
        }
    }
}
