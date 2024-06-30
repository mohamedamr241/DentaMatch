using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Add_Dental_Disease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Caries" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Fixed Prosthodontics Crown and Brigde" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Fixed Prosthodontics Implantology" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Partial Removable Prosthodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Complete Removable Prosthodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Orthodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Gingivitis - periodontitis" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Endodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Dental abscess" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM DentalDiseases");
        }
    }
}
