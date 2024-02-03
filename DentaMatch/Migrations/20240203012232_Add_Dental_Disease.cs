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
                values: new object[] { Guid.NewGuid().ToString(), "Gingivitis" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Prosthodontic" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Gumboil" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Dental abscess" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Orthodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Displaced tooth" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Implantology" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Endodontics" }
            );
            migrationBuilder.InsertData(
                table: "DentalDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Edentulous" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM DentalDiseases");
        }
    }
}
