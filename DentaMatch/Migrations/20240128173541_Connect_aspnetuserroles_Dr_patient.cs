using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Connect_aspnetuserroles_Dr_patient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "DoctorId",
            table: "AspNetUserRoles",
            nullable: true,
            defaultValue: null);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_DoctorId",
                table: "AspNetUserRoles",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Doctors_DoctorId",
                table: "AspNetUserRoles",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddColumn<int>(
            name: "PatientId",
            table: "AspNetUserRoles",
            nullable: true,
            defaultValue: null);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_PatientId",
                table: "AspNetUserRoles",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Patients_PatientId",
                table: "AspNetUserRoles",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_AspNetUserRoles_Doctors_DoctorId",
            table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_DoctorId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
            name: "FK_AspNetUserRoles_Patients_PatientId",
            table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_PatientId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "AspNetUserRoles");
        }
    }
}
