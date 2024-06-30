using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Dental_case_progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalCaseProgress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProgressMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgressDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalCaseProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalCaseProgress_DentalCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "DentalCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalCaseProgress_CaseId",
                table: "DentalCaseProgress",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalCaseProgress");
        }
    }
}
