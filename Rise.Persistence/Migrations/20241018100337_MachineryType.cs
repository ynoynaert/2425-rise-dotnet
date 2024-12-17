using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MachineryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MachineryTypeId",
                table: "Machinery",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MachineryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineryTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machinery_MachineryTypeId",
                table: "Machinery",
                column: "MachineryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Machinery_MachineryTypes_MachineryTypeId",
                table: "Machinery",
                column: "MachineryTypeId",
                principalTable: "MachineryTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machinery_MachineryTypes_MachineryTypeId",
                table: "Machinery");

            migrationBuilder.DropTable(
                name: "MachineryTypes");

            migrationBuilder.DropIndex(
                name: "IX_Machinery_MachineryTypeId",
                table: "Machinery");

            migrationBuilder.DropColumn(
                name: "MachineryTypeId",
                table: "Machinery");
        }
    }
}
