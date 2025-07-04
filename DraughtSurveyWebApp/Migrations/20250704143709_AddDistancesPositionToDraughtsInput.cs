using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDistancesPositionToDraughtsInput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsLCFForward",
                table: "HydrostaticInputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAftDistanceToFwd",
                table: "DraughtsInputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isFwdDistancetoFwd",
                table: "DraughtsInputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMidDistanceToFwd",
                table: "DraughtsInputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAftDistanceToFwd",
                table: "DraughtsInputs");

            migrationBuilder.DropColumn(
                name: "isFwdDistancetoFwd",
                table: "DraughtsInputs");

            migrationBuilder.DropColumn(
                name: "isMidDistanceToFwd",
                table: "DraughtsInputs");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLCFForward",
                table: "HydrostaticInputs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");
        }
    }
}
