using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DraughtSurveyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoginCount = table.Column<int>(type: "integer", nullable: false),
                    AdminNote = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                name: "ExcelTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: true),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcelTemplates_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VesselName = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<string>(type: "text", nullable: true),
                    CompanyReference = table.Column<string>(type: "text", nullable: true),
                    OperationType = table.Column<int>(type: "integer", nullable: true),
                    notShowInputWarnings = table.Column<bool>(type: "boolean", nullable: false),
                    notApplyAutoFillingHydrostatics = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserHydrostaticTableHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IMO = table.Column<string>(type: "text", nullable: false),
                    VesselName = table.Column<string>(type: "text", nullable: false),
                    TableStep = table.Column<double>(type: "double precision", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHydrostaticTableHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHydrostaticTableHeaders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    Ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExcelExportLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    TemplateId = table.Column<int>(type: "integer", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelExportLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcelExportLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExcelExportLogs_ExcelTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ExcelTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CargoInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CargoName = table.Column<string>(type: "text", nullable: true),
                    DeclaredWeight = table.Column<double>(type: "double precision", nullable: true),
                    Shipper = table.Column<string>(type: "text", nullable: true),
                    Consignee = table.Column<string>(type: "text", nullable: true),
                    LoadingTerminal = table.Column<string>(type: "text", nullable: true),
                    BerthNumber = table.Column<string>(type: "text", nullable: true),
                    DischargingPort = table.Column<string>(type: "text", nullable: true),
                    InspectionId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CargoByDraughtSurvey = table.Column<double>(type: "double precision", nullable: true),
                    DifferenceWithBL_Mt = table.Column<double>(type: "double precision", nullable: true),
                    DifferenceWithBL_Percents = table.Column<double>(type: "double precision", nullable: true),
                    DifferenceWithSDWT_Percents = table.Column<double>(type: "double precision", nullable: true),
                    InspectionId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SurveyType = table.Column<int>(type: "integer", nullable: false),
                    SurveyTimeStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SurveyTimeEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CargoOperationsDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    InspectionId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IMO = table.Column<string>(type: "text", nullable: false),
                    LBP = table.Column<double>(type: "double precision", nullable: true),
                    BM = table.Column<double>(type: "double precision", nullable: true),
                    LS = table.Column<double>(type: "double precision", nullable: true),
                    SDWT = table.Column<double>(type: "double precision", nullable: true),
                    DeclaredConstant = table.Column<double>(type: "double precision", nullable: true),
                    InspectionId = table.Column<int>(type: "integer", nullable: false)
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
                name: "UserHydrostaticTableRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Draught = table.Column<double>(type: "double precision", nullable: true),
                    Displacement = table.Column<double>(type: "double precision", nullable: true),
                    TPC = table.Column<double>(type: "double precision", nullable: true),
                    LCF = table.Column<double>(type: "double precision", nullable: true),
                    IsLcfForward = table.Column<bool>(type: "boolean", nullable: true),
                    MTCPlus50 = table.Column<double>(type: "double precision", nullable: true),
                    MTCMinus50 = table.Column<double>(type: "double precision", nullable: true),
                    UserHydrostaticTableHeaderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHydrostaticTableRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHydrostaticTableRows_UserHydrostaticTableHeaders_UserHy~",
                        column: x => x.UserHydrostaticTableHeaderId,
                        principalTable: "UserHydrostaticTableHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeductiblesInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ballast = table.Column<double>(type: "double precision", nullable: true),
                    FreshWater = table.Column<double>(type: "double precision", nullable: true),
                    FuelOil = table.Column<double>(type: "double precision", nullable: true),
                    DieselOil = table.Column<double>(type: "double precision", nullable: true),
                    LubOil = table.Column<double>(type: "double precision", nullable: true),
                    Others = table.Column<double>(type: "double precision", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalDeductibles = table.Column<double>(type: "double precision", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Swell = table.Column<double>(type: "double precision", nullable: true),
                    DraughtFwdPS = table.Column<double>(type: "double precision", nullable: true),
                    DraughtFwdSS = table.Column<double>(type: "double precision", nullable: true),
                    DraughtMidPS = table.Column<double>(type: "double precision", nullable: true),
                    DraughtMidSS = table.Column<double>(type: "double precision", nullable: true),
                    DraughtAftPS = table.Column<double>(type: "double precision", nullable: true),
                    DraughtAftSS = table.Column<double>(type: "double precision", nullable: true),
                    BreadthForward = table.Column<double>(type: "double precision", nullable: true),
                    BreadthAft = table.Column<double>(type: "double precision", nullable: true),
                    DistanceFwd = table.Column<double>(type: "double precision", nullable: true),
                    DistanceMid = table.Column<double>(type: "double precision", nullable: true),
                    DistanceAft = table.Column<double>(type: "double precision", nullable: true),
                    isFwdDistancetoFwd = table.Column<bool>(type: "boolean", nullable: true),
                    isMidDistanceToFwd = table.Column<bool>(type: "boolean", nullable: true),
                    isAftDistanceToFwd = table.Column<bool>(type: "boolean", nullable: true),
                    SeaWaterDensity = table.Column<double>(type: "double precision", nullable: true),
                    KeelCorrection = table.Column<double>(type: "double precision", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DraughtMeanFwd = table.Column<double>(type: "double precision", nullable: true),
                    DraughtMeanMid = table.Column<double>(type: "double precision", nullable: true),
                    DraughtMeanAft = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectionFwd = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectionMid = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectionAft = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectedFwd = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectedMid = table.Column<double>(type: "double precision", nullable: true),
                    DraughtCorrectedAft = table.Column<double>(type: "double precision", nullable: true),
                    TrimApparent = table.Column<double>(type: "double precision", nullable: true),
                    HoggingSagging = table.Column<double>(type: "double precision", nullable: true),
                    Heel = table.Column<double>(type: "double precision", nullable: true),
                    LBD = table.Column<double>(type: "double precision", nullable: true),
                    TrimCorrected = table.Column<double>(type: "double precision", nullable: true),
                    MeanAdjustedDraught = table.Column<double>(type: "double precision", nullable: true),
                    MeanAdjustedDraughtAfterKeelCorrection = table.Column<double>(type: "double precision", nullable: true),
                    MeanFwdAftDraught = table.Column<double>(type: "double precision", nullable: true),
                    MeanOfMeanDraught = table.Column<double>(type: "double precision", nullable: true),
                    DifferenceMTC1MTC2 = table.Column<double>(type: "double precision", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DraughtAbove = table.Column<double>(type: "double precision", nullable: true),
                    DraughtBelow = table.Column<double>(type: "double precision", nullable: true),
                    DisplacementAbove = table.Column<double>(type: "double precision", nullable: true),
                    TPCAbove = table.Column<double>(type: "double precision", nullable: true),
                    LCFAbove = table.Column<double>(type: "double precision", nullable: true),
                    MTCPlus50Above = table.Column<double>(type: "double precision", nullable: true),
                    MTCMinus50Above = table.Column<double>(type: "double precision", nullable: true),
                    LCFfromAftAbove = table.Column<double>(type: "double precision", nullable: true),
                    DisplacementBelow = table.Column<double>(type: "double precision", nullable: true),
                    TPCBelow = table.Column<double>(type: "double precision", nullable: true),
                    LCFBelow = table.Column<double>(type: "double precision", nullable: true),
                    MTCPlus50Below = table.Column<double>(type: "double precision", nullable: true),
                    MTCMinus50Below = table.Column<double>(type: "double precision", nullable: true),
                    LCFfromAftBelow = table.Column<double>(type: "double precision", nullable: true),
                    IsLCFForward = table.Column<bool>(type: "boolean", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplacementFromTable = table.Column<double>(type: "double precision", nullable: true),
                    TPCFromTable = table.Column<double>(type: "double precision", nullable: true),
                    LCFFromTable = table.Column<double>(type: "double precision", nullable: true),
                    MTCPlus50FromTable = table.Column<double>(type: "double precision", nullable: true),
                    MTCMinus50FromTable = table.Column<double>(type: "double precision", nullable: true),
                    FirstTrimCorrection = table.Column<double>(type: "double precision", nullable: true),
                    SecondTrimCorrection = table.Column<double>(type: "double precision", nullable: true),
                    DisplacementCorrectedForTrim = table.Column<double>(type: "double precision", nullable: true),
                    DisplacementCorrectedForDensity = table.Column<double>(type: "double precision", nullable: true),
                    NettoDisplacement = table.Column<double>(type: "double precision", nullable: true),
                    CargoPlusConstant = table.Column<double>(type: "double precision", nullable: true),
                    DraughtSurveyBlockId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_ExcelExportLogs_CreatedUtc",
                table: "ExcelExportLogs",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelExportLogs_TemplateId",
                table: "ExcelExportLogs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelExportLogs_UserId",
                table: "ExcelExportLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelTemplates_Name",
                table: "ExcelTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelTemplates_OwnerId",
                table: "ExcelTemplates",
                column: "OwnerId");

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
                name: "IX_Inspections_ApplicationUserId",
                table: "Inspections",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHydrostaticTableHeaders_ApplicationUserId",
                table: "UserHydrostaticTableHeaders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHydrostaticTableRows_UserHydrostaticTableHeaderId",
                table: "UserHydrostaticTableRows",
                column: "UserHydrostaticTableHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_LastSeenUtc",
                table: "UserSessions",
                column: "LastSeenUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_LastSeenUtc",
                table: "UserSessions",
                columns: new[] { "UserId", "LastSeenUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_StartedUtc",
                table: "UserSessions",
                columns: new[] { "UserId", "StartedUtc" });

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
                name: "ExcelExportLogs");

            migrationBuilder.DropTable(
                name: "HydrostaticInputs");

            migrationBuilder.DropTable(
                name: "HydrostaticResults");

            migrationBuilder.DropTable(
                name: "UserHydrostaticTableRows");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "VesselInputs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ExcelTemplates");

            migrationBuilder.DropTable(
                name: "DraughtSurveyBlocks");

            migrationBuilder.DropTable(
                name: "UserHydrostaticTableHeaders");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
