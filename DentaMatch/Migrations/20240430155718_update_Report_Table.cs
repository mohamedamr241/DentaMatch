using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class update_Report_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
