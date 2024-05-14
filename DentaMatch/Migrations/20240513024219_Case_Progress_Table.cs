using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Case_Progress_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
         name: "DentalCaseProgresses",
         columns: table => new
         {
             Id = table.Column<string>(nullable: false),
             DoctorId = table.Column<string>(nullable: false),
             CaseId = table.Column<string>(nullable: false),
             ProgressMessage = table.Column<string>(nullable: false),
             ProgressDate = table.Column<DateTime>(nullable: false)
         },
         constraints: table =>
         {
             table.PrimaryKey("PK_DentalCaseProgresses", x => x.Id);
             table.ForeignKey(
                 name: "FK_DentalCaseProgresses_DentalCases_CaseId",
                 column: x => x.CaseId,
                 principalTable: "DentalCases",
                 principalColumn: "Id",
                 onDelete: ReferentialAction.Restrict); 
             table.ForeignKey(
                 name: "FK_DentalCaseProgresses_Doctors_DoctorId",
                 column: x => x.DoctorId,
                 principalTable: "Doctors",
                 principalColumn: "Id",
                 onDelete: ReferentialAction.Restrict); 
         });

            migrationBuilder.CreateIndex(
                name: "IX_DentalCaseProgresses_CaseId",
                table: "DentalCaseProgresses",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalCaseProgresses_DoctorId",
                table: "DentalCaseProgresses",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalCaseProgresses");
        }
    }
}
