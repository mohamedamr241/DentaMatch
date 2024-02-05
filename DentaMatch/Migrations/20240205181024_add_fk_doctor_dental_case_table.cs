using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class add_fk_doctor_dental_case_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalCases_Patients_PatientId",
                table: "DentalCases");

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "DentalCases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DentalCases_DoctorId",
                table: "DentalCases",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalCases_Patients_PatientId",
                table: "DentalCases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases");

            migrationBuilder.DropForeignKey(
                name: "FK_DentalCases_Patients_PatientId",
                table: "DentalCases");

            migrationBuilder.DropIndex(
                name: "IX_DentalCases_DoctorId",
                table: "DentalCases");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "DentalCases");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalCases_Patients_PatientId",
                table: "DentalCases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
