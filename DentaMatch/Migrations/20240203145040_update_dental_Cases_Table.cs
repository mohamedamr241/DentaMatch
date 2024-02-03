using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class update_dental_Cases_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CaseDentalDiseases_CaseId",
                table: "CaseDentalDiseases");

            migrationBuilder.DropIndex(
                name: "IX_CaseChronicDiseases_CaseId",
                table: "CaseChronicDiseases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseDentalDiseases",
                table: "CaseDentalDiseases",
                columns: new[] { "CaseId", "DiseaseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseChronicDiseases",
                table: "CaseChronicDiseases",
                columns: new[] { "CaseId", "DiseaseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseDentalDiseases",
                table: "CaseDentalDiseases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseChronicDiseases",
                table: "CaseChronicDiseases");

            migrationBuilder.CreateIndex(
                name: "IX_CaseDentalDiseases_CaseId",
                table: "CaseDentalDiseases",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseChronicDiseases_CaseId",
                table: "CaseChronicDiseases",
                column: "CaseId");
        }
    }
}
