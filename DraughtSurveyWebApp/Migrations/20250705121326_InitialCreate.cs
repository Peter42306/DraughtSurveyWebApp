using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LoginCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminNote = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VesselName = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyReference = table.Column<string>(type: "TEXT", nullable: true),
                    OperationType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargoInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CargoName = table.Column<string>(type: "TEXT", nullable: true),
                    DeclaredWeight = table.Column<double>(type: "REAL", nullable: true),
                    LoadingTerminal = table.Column<string>(type: "TEXT", nullable: true),
                    BerthNumber = table.Column<string>(type: "TEXT", nullable: true),
                    InspectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoInputs_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargoResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CargoByDraughtSurvey = table.Column<double>(type: "REAL", nullable: true),
                    DifferenceWithBL_Mt = table.Column<double>(type: "REAL", nullable: true),
                    DifferenceWithBL_Percents = table.Column<double>(type: "REAL", nullable: true),
                    DifferenceWithSDWT_Percents = table.Column<double>(type: "REAL", nullable: true),
                    InspectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoResults_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraughtSurveyBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyType = table.Column<int>(type: "INTEGER", nullable: false),
                    SurveyTimeStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SurveyTimeEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CargoOperationsDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    InspectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraughtSurveyBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraughtSurveyBlocks_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VesselInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IMO = table.Column<string>(type: "TEXT", nullable: false),
                    LBP = table.Column<double>(type: "REAL", nullable: true),
                    BM = table.Column<double>(type: "REAL", nullable: true),
                    LS = table.Column<double>(type: "REAL", nullable: true),
                    SDWT = table.Column<double>(type: "REAL", nullable: true),
                    DeclaredConstant = table.Column<double>(type: "REAL", nullable: true),
                    InspectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselInputs_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeductiblesInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ballast = table.Column<double>(type: "REAL", nullable: true),
                    FreshWater = table.Column<double>(type: "REAL", nullable: true),
                    FuelOil = table.Column<double>(type: "REAL", nullable: true),
                    DieselOil = table.Column<double>(type: "REAL", nullable: true),
                    LubOil = table.Column<double>(type: "REAL", nullable: true),
                    Others = table.Column<double>(type: "REAL", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductiblesInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeductiblesInputs_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeductiblesResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TotalDeductibles = table.Column<double>(type: "REAL", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductiblesResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeductiblesResults_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraughtsInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DraughtFwdPS = table.Column<double>(type: "REAL", nullable: true),
                    DraughtFwdSS = table.Column<double>(type: "REAL", nullable: true),
                    DraughtMidPS = table.Column<double>(type: "REAL", nullable: true),
                    DraughtMidSS = table.Column<double>(type: "REAL", nullable: true),
                    DraughtAftPS = table.Column<double>(type: "REAL", nullable: true),
                    DraughtAftSS = table.Column<double>(type: "REAL", nullable: true),
                    DistanceFwd = table.Column<double>(type: "REAL", nullable: true),
                    DistanceMid = table.Column<double>(type: "REAL", nullable: true),
                    DistanceAft = table.Column<double>(type: "REAL", nullable: true),
                    isFwdDistancetoFwd = table.Column<bool>(type: "INTEGER", nullable: true),
                    isMidDistanceToFwd = table.Column<bool>(type: "INTEGER", nullable: true),
                    isAftDistanceToFwd = table.Column<bool>(type: "INTEGER", nullable: true),
                    SeaWaterDensity = table.Column<double>(type: "REAL", nullable: true),
                    KeelCorrection = table.Column<double>(type: "REAL", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraughtsInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraughtsInputs_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraughtsResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DraughtMeanFwd = table.Column<double>(type: "REAL", nullable: true),
                    DraughtMeanMid = table.Column<double>(type: "REAL", nullable: true),
                    DraughtMeanAft = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectionFwd = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectionMid = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectionAft = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectedFwd = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectedMid = table.Column<double>(type: "REAL", nullable: true),
                    DraughtCorrectedAft = table.Column<double>(type: "REAL", nullable: true),
                    TrimApparent = table.Column<double>(type: "REAL", nullable: true),
                    HoggingSagging = table.Column<double>(type: "REAL", nullable: true),
                    Heel = table.Column<double>(type: "REAL", nullable: true),
                    TrimCorrected = table.Column<double>(type: "REAL", nullable: true),
                    MeanAdjustedDraught = table.Column<double>(type: "REAL", nullable: true),
                    MeanAdjustedDraughtAfterKeelCorrection = table.Column<double>(type: "REAL", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraughtsResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraughtsResults_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HydrostaticInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DraughtAbove = table.Column<double>(type: "REAL", nullable: true),
                    DraughtBelow = table.Column<double>(type: "REAL", nullable: true),
                    DisplacementAbove = table.Column<double>(type: "REAL", nullable: true),
                    TPCAbove = table.Column<double>(type: "REAL", nullable: true),
                    LCFAbove = table.Column<double>(type: "REAL", nullable: true),
                    MTCPlus50Above = table.Column<double>(type: "REAL", nullable: true),
                    MTCMinus50Above = table.Column<double>(type: "REAL", nullable: true),
                    LCFfromAftAbove = table.Column<double>(type: "REAL", nullable: true),
                    DisplacementBelow = table.Column<double>(type: "REAL", nullable: true),
                    TPCBelow = table.Column<double>(type: "REAL", nullable: true),
                    LCFBelow = table.Column<double>(type: "REAL", nullable: true),
                    MTCPlus50Below = table.Column<double>(type: "REAL", nullable: true),
                    MTCMinus50Below = table.Column<double>(type: "REAL", nullable: true),
                    LCFfromAftBelow = table.Column<double>(type: "REAL", nullable: true),
                    IsLCFForward = table.Column<bool>(type: "INTEGER", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HydrostaticInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HydrostaticInputs_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HydrostaticResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplacementFromTable = table.Column<double>(type: "REAL", nullable: true),
                    TPCFromTable = table.Column<double>(type: "REAL", nullable: true),
                    LCFFromTable = table.Column<double>(type: "REAL", nullable: true),
                    MTCPlus50FromTable = table.Column<double>(type: "REAL", nullable: true),
                    MTCMinus50FromTable = table.Column<double>(type: "REAL", nullable: true),
                    FirstTrimCorrection = table.Column<double>(type: "REAL", nullable: true),
                    SecondTrimCorrection = table.Column<double>(type: "REAL", nullable: true),
                    DisplacementCorrectedForTrim = table.Column<double>(type: "REAL", nullable: true),
                    DisplacementCorrectedForDensity = table.Column<double>(type: "REAL", nullable: true),
                    NettoDisplacement = table.Column<double>(type: "REAL", nullable: true),
                    CargoPlusConstant = table.Column<double>(type: "REAL", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HydrostaticResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HydrostaticResults_DraughtSurveyBlocks_DraughtSurveyBlockId",
                        column: x => x.DraughtSurveyBlockId,
                        principalTable: "DraughtSurveyBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoInputs_InspectionId",
                table: "CargoInputs",
                column: "InspectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoResults_InspectionId",
                table: "CargoResults",
                column: "InspectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeductiblesInputs_DraughtSurveyBlockId",
                table: "DeductiblesInputs",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeductiblesResults_DraughtSurveyBlockId",
                table: "DeductiblesResults",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DraughtsInputs_DraughtSurveyBlockId",
                table: "DraughtsInputs",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DraughtsResults_DraughtSurveyBlockId",
                table: "DraughtsResults",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DraughtSurveyBlocks_InspectionId",
                table: "DraughtSurveyBlocks",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_HydrostaticInputs_DraughtSurveyBlockId",
                table: "HydrostaticInputs",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HydrostaticResults_DraughtSurveyBlockId",
                table: "HydrostaticResults",
                column: "DraughtSurveyBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VesselInputs_InspectionId",
                table: "VesselInputs",
                column: "InspectionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CargoInputs");

            migrationBuilder.DropTable(
                name: "CargoResults");

            migrationBuilder.DropTable(
                name: "DeductiblesInputs");

            migrationBuilder.DropTable(
                name: "DeductiblesResults");

            migrationBuilder.DropTable(
                name: "DraughtsInputs");

            migrationBuilder.DropTable(
                name: "DraughtsResults");

            migrationBuilder.DropTable(
                name: "HydrostaticInputs");

            migrationBuilder.DropTable(
                name: "HydrostaticResults");

            migrationBuilder.DropTable(
                name: "VesselInputs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DraughtSurveyBlocks");

            migrationBuilder.DropTable(
                name: "Inspections");
        }
    }
}
