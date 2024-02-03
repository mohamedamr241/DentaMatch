using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class Add_Chronic_Disease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Diabetes" }
            );
            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Cardiovascular" }
            );
            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Blood Pressure" }
            );
            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Heart Disease" }
            );
            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "Id", "DiseaseName" },
                values: new object[] { Guid.NewGuid().ToString(), "Cancer" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ChronicDiseases");
        }
    }
}
