using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Customers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Quotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Quotes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "QuoteNumber",
                table: "Quotes",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWithVat",
                table: "Quotes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWithoutVat",
                table: "Quotes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "QuoteId",
                table: "MachineryOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StreetNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    City = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineryOption_QuoteId",
                table: "MachineryOption",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineryOption_Quotes_QuoteId",
                table: "MachineryOption",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Customers_CustomerId",
                table: "Quotes",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Quotes_QuoteId",
                table: "MachineryOption");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Customers_CustomerId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_MachineryOption_QuoteId",
                table: "MachineryOption");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "QuoteNumber",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "TotalWithVat",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "TotalWithoutVat",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "MachineryOption");
        }
    }
}
