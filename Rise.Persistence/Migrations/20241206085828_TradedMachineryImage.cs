using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TradedMachineryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_TradedMachineries_TradedMachineryId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_TradedMachineryId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "TradedMachineryId",
                table: "Image");

            migrationBuilder.CreateTable(
                name: "TradedMachineryImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradedMachineryId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradedMachineryImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradedMachineryImage_TradedMachineries_TradedMachineryId",
                        column: x => x.TradedMachineryId,
                        principalTable: "TradedMachineries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradedMachineryImage_TradedMachineryId",
                table: "TradedMachineryImage",
                column: "TradedMachineryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradedMachineryImage");

            migrationBuilder.AddColumn<int>(
                name: "TradedMachineryId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_TradedMachineryId",
                table: "Image",
                column: "TradedMachineryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_TradedMachineries_TradedMachineryId",
                table: "Image",
                column: "TradedMachineryId",
                principalTable: "TradedMachineries",
                principalColumn: "Id");
        }
    }
}
