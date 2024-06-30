using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class ReportCountColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumOfReport",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfReport",
                table: "AspNetUsers");
        }
    }
}
