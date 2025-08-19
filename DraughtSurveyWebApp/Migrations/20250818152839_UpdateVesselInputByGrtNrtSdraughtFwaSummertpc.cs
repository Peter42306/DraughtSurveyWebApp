using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVesselInputByGrtNrtSdraughtFwaSummertpc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FWA",
                table: "VesselInputs",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GRT",
                table: "VesselInputs",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NRT",
                table: "VesselInputs",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SummerDraught",
                table: "VesselInputs",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SummerTPC",
                table: "VesselInputs",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FWA",
                table: "VesselInputs");

            migrationBuilder.DropColumn(
                name: "GRT",
                table: "VesselInputs");

            migrationBuilder.DropColumn(
                name: "NRT",
                table: "VesselInputs");

            migrationBuilder.DropColumn(
                name: "SummerDraught",
                table: "VesselInputs");

            migrationBuilder.DropColumn(
                name: "SummerTPC",
                table: "VesselInputs");
        }
    }
}
