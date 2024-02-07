using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Update_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases");

            migrationBuilder.RenameColumn(
                name: "Government",
                table: "AspNetUsers",
                newName: "City");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patients",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "AspNetUsers",
                newName: "Government");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalCases_Doctors_DoctorId",
                table: "DentalCases",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
