using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentaMatch.Migrations
{
    /// <inheritdoc />
    public partial class make_PhoneNumber_Required_Identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "char(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: null,
                oldType: "nvarchar(max)", // Specify the old type if necessary
                oldNullable: true // Specify oldNullable if necessary
            );
            migrationBuilder.Sql("ALTER TABLE AspNetUsers ADD CONSTRAINT CK_NumericPhoneNumber CHECK (ISNUMERIC(PhoneNumber) = 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE AspNetUsers DROP CONSTRAINT CK_NumericPhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(11)");

        }
    }
}
