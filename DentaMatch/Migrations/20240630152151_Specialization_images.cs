using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Specialization_images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "DoctorSpecialization",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "DoctorSpecialization",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "DoctorSpecialization");

            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "DoctorSpecialization");
        }
    }
}
