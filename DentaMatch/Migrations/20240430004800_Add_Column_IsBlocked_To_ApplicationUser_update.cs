using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_IsBlocked_To_ApplicationUser_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_DentalCases_CaseId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Doctors_DoctorId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_DentalCases_CaseId",
                table: "Report",
                column: "CaseId",
                principalTable: "DentalCases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Doctors_DoctorId",
                table: "Report",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

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
                name: "FK_Report_DentalCases_CaseId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Doctors_DoctorId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_DentalCases_CaseId",
                table: "Report",
                column: "CaseId",
                principalTable: "DentalCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Doctors_DoctorId",
                table: "Report",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Patients_PatientId",
                table: "Report",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
