using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace VisionApplication.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cameraParameterModel",
                columns: table => new
                {
                    cameraID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    softwareTrigger = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    exposureTime = table.Column<float>(type: "float", nullable: false),
                    frameRate = table.Column<float>(type: "float", nullable: false),
                    gain = table.Column<float>(type: "float", nullable: false),
                    dateChanged = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cameraParameterModel", x => x.cameraID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rectangles ROI",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    left = table.Column<double>(type: "double", nullable: false),
                    top = table.Column<double>(type: "double", nullable: false),
                    Width = table.Column<double>(type: "double", nullable: false),
                    Height = table.Column<double>(type: "double", nullable: false),
                    Angle = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rectangles ROI", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categoryTeachParametersModel",
                columns: table => new
                {
                    cameraID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    L_DeviceLocationRoiId = table.Column<int>(type: "int", nullable: false),
                    L_LocationEnable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    L_ThresholdType = table.Column<int>(type: "int", nullable: false),
                    L_ObjectColor = table.Column<int>(type: "int", nullable: false),
                    L_lowerThreshold = table.Column<int>(type: "int", nullable: false),
                    L_upperThreshold = table.Column<int>(type: "int", nullable: false),
                    L_lowerThresholdInnerChip = table.Column<int>(type: "int", nullable: false),
                    L_upperThresholdInnerChip = table.Column<int>(type: "int", nullable: false),
                    L_OpeningMask = table.Column<int>(type: "int", nullable: false),
                    L_DilationMask = table.Column<int>(type: "int", nullable: false),
                    L_MinWidthDevice = table.Column<int>(type: "int", nullable: false),
                    L_MinHeightDevice = table.Column<int>(type: "int", nullable: false),
                    L_TemplateRoiId = table.Column<int>(type: "int", nullable: false),
                    L_NumberSide = table.Column<int>(type: "int", nullable: false),
                    L_ScaleImageRatio = table.Column<double>(type: "double", nullable: false),
                    L_MinScore = table.Column<double>(type: "double", nullable: false),
                    L_CornerIndex = table.Column<int>(type: "int", nullable: false),
                    L_NumberROILocation = table.Column<int>(type: "int", nullable: false),
                    OC_EnableCheck = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OC_lowerThreshold = table.Column<int>(type: "int", nullable: false),
                    OC_upperThreshold = table.Column<int>(type: "int", nullable: false),
                    OC_OpeningMask = table.Column<int>(type: "int", nullable: false),
                    OC_DilationMask = table.Column<int>(type: "int", nullable: false),
                    OC_MinWidthDevice = table.Column<int>(type: "int", nullable: false),
                    OC_MinHeightDevice = table.Column<int>(type: "int", nullable: false),
                    dateChanged = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    teachImage = table.Column<byte[]>(type: "longblob", nullable: false),
                    templateImage = table.Column<byte[]>(type: "longblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoryTeachParametersModel", x => x.cameraID);
                    table.ForeignKey(
                        name: "FK_categoryTeachParametersModel_Rectangles ROI_L_DeviceLocation~",
                        column: x => x.L_DeviceLocationRoiId,
                        principalTable: "Rectangles ROI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_categoryTeachParametersModel_Rectangles ROI_L_TemplateRoiId",
                        column: x => x.L_TemplateRoiId,
                        principalTable: "Rectangles ROI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categoryVisionParametersModel",
                columns: table => new
                {
                    areaID = table.Column<int>(type: "int", nullable: false),
                    cameraID = table.Column<int>(type: "int", nullable: false),
                    LD_DefectROILocationId = table.Column<int>(type: "int", nullable: false),
                    LD_AreaEnable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LD_lowerThreshold = table.Column<int>(type: "int", nullable: false),
                    LD_upperThreshold = table.Column<int>(type: "int", nullable: false),
                    LD_OpeningMask = table.Column<int>(type: "int", nullable: false),
                    LD_DilationMask = table.Column<int>(type: "int", nullable: false),
                    LD_ObjectCoverPercent = table.Column<int>(type: "int", nullable: false),
                    dateChanged = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoryVisionParametersModel", x => new { x.cameraID, x.areaID });
                    table.ForeignKey(
                        name: "FK_categoryVisionParametersModel_Rectangles ROI_LD_DefectROILoc~",
                        column: x => x.LD_DefectROILocationId,
                        principalTable: "Rectangles ROI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_categoryVisionParametersModel_categoryTeachParametersModel_c~",
                        column: x => x.cameraID,
                        principalTable: "categoryTeachParametersModel",
                        principalColumn: "cameraID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_categoryTeachParametersModel_L_DeviceLocationRoiId",
                table: "categoryTeachParametersModel",
                column: "L_DeviceLocationRoiId");

            migrationBuilder.CreateIndex(
                name: "IX_categoryTeachParametersModel_L_TemplateRoiId",
                table: "categoryTeachParametersModel",
                column: "L_TemplateRoiId");

            migrationBuilder.CreateIndex(
                name: "IX_categoryVisionParametersModel_LD_DefectROILocationId",
                table: "categoryVisionParametersModel",
                column: "LD_DefectROILocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cameraParameterModel");

            migrationBuilder.DropTable(
                name: "categoryVisionParametersModel");

            migrationBuilder.DropTable(
                name: "categoryTeachParametersModel");

            migrationBuilder.DropTable(
                name: "Rectangles ROI");
        }
    }
}
