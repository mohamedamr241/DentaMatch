using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class updatepatientdentalcaseconstrain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE DentalCases DROP CONSTRAINT FK_DentalCases_Patients_PatientId");
            migrationBuilder.Sql("ALTER TABLE DentalCases ADD CONSTRAINT FK_DentalCases_Patients_PatientId FOREIGN KEY (PatientId) REFERENCES [dbo].[Patients]([Id]) ON DELETE CASCADE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE DentalCases DROP CONSTRAINT FK_DentalCases_Patients_PatientId");
        }
    }
}
