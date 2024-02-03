using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Create_Dental_Case_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChronicDiseases",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiseaseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicDiseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DentalCases",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalCases_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalDiseases",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiseaseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalDiseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseChronicDiseases",
                columns: table => new
                {
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiseaseId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CaseChronicDiseases_ChronicDiseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "ChronicDiseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseChronicDiseases_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MouthImages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouthImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouthImages_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionImages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionImages_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XrayIamges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XrayIamges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XrayIamges_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseDentalDiseases",
                columns: table => new
                {
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiseaseId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CaseDentalDiseases_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseDentalDiseases_DentalDiseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "DentalDiseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseChronicDiseases_CaseId",
                table: "CaseChronicDiseases",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseChronicDiseases_DiseaseId",
                table: "CaseChronicDiseases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseDentalDiseases_CaseId",
                table: "CaseDentalDiseases",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseDentalDiseases_DiseaseId",
                table: "CaseDentalDiseases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalCases_PatientId",
                table: "DentalCases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MouthImages_CaseId",
                table: "MouthImages",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionImages_CaseId",
                table: "PrescriptionImages",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_XrayIamges_CaseId",
                table: "XrayIamges",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseChronicDiseases");

            migrationBuilder.DropTable(
                name: "CaseDentalDiseases");

            migrationBuilder.DropTable(
                name: "MouthImages");

            migrationBuilder.DropTable(
                name: "PrescriptionImages");

            migrationBuilder.DropTable(
                name: "XrayIamges");

            migrationBuilder.DropTable(
                name: "ChronicDiseases");

            migrationBuilder.DropTable(
                name: "DentalDiseases");

            migrationBuilder.DropTable(
                name: "DentalCases");
        }
    }
}
