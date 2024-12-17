using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TradedMachinery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TradedMachineryId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TradedMachineries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    QuoteId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradedMachineries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradedMachineries_MachineryType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "MachineryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradedMachineries_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_TradedMachineryId",
                table: "Image",
                column: "TradedMachineryId");

            migrationBuilder.CreateIndex(
                name: "IX_TradedMachineries_QuoteId",
                table: "TradedMachineries",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TradedMachineries_TypeId",
                table: "TradedMachineries",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_TradedMachineries_TradedMachineryId",
                table: "Image",
                column: "TradedMachineryId",
                principalTable: "TradedMachineries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_TradedMachineries_TradedMachineryId",
                table: "Image");

            migrationBuilder.DropTable(
                name: "TradedMachineries");

            migrationBuilder.DropIndex(
                name: "IX_Image_TradedMachineryId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "TradedMachineryId",
                table: "Image");
        }
    }
}
