using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Option_Code",
                table: "Option",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machinery_SerialNumber",
                table: "Machinery",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Code",
                table: "Category",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Option_Code",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Machinery_SerialNumber",
                table: "Machinery");

            migrationBuilder.DropIndex(
                name: "IX_Category_Code",
                table: "Category");
        }
    }
}
