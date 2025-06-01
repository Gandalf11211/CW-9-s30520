using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CW_9_s30520.Migrations
{
    /// <inheritdoc />
    public partial class submitchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Patient",
                newName: "BirthDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Patient",
                newName: "DateOfBirth");
        }
    }
}
