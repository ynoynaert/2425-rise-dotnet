using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Quote_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Quotes_QuoteId",
                table: "MachineryOption");

            migrationBuilder.DropIndex(
                name: "IX_MachineryOption_QuoteId",
                table: "MachineryOption");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "MachineryOption");

            migrationBuilder.AddColumn<int>(
                name: "MachineryId",
                table: "Quotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuoteOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteId = table.Column<int>(type: "int", nullable: false),
                    MachineryOptionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteOptions_MachineryOption_MachineryOptionId",
                        column: x => x.MachineryOptionId,
                        principalTable: "MachineryOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteOptions_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_MachineryId",
                table: "Quotes",
                column: "MachineryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteOptions_MachineryOptionId",
                table: "QuoteOptions",
                column: "MachineryOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteOptions_QuoteId",
                table: "QuoteOptions",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Machinery_MachineryId",
                table: "Quotes",
                column: "MachineryId",
                principalTable: "Machinery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Machinery_MachineryId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "QuoteOptions");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_MachineryId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "MachineryId",
                table: "Quotes");

            migrationBuilder.AddColumn<int>(
                name: "QuoteId",
                table: "MachineryOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
