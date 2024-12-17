using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inquiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Machinery_MachineryId",
                table: "MachineryOption");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Option_OptionId",
                table: "MachineryOption");

            migrationBuilder.CreateTable(
                name: "Inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    MachineryId = table.Column<int>(type: "int", nullable: false),
                    SalespersonId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inquiries_Machinery_MachineryId",
                        column: x => x.MachineryId,
                        principalTable: "Machinery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InquiryOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryId = table.Column<int>(type: "int", nullable: false),
                    MachineryOptionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryOptions_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquiryOptions_MachineryOption_MachineryOptionId",
                        column: x => x.MachineryOptionId,
                        principalTable: "MachineryOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_MachineryId",
                table: "Inquiries",
                column: "MachineryId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryOptions_InquiryId",
                table: "InquiryOptions",
                column: "InquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryOptions_MachineryOptionId",
                table: "InquiryOptions",
                column: "MachineryOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineryOption_Machinery_MachineryId",
                table: "MachineryOption",
                column: "MachineryId",
                principalTable: "Machinery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineryOption_Option_OptionId",
                table: "MachineryOption",
                column: "OptionId",
                principalTable: "Option",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Machinery_MachineryId",
                table: "MachineryOption");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineryOption_Option_OptionId",
                table: "MachineryOption");

            migrationBuilder.DropTable(
                name: "InquiryOptions");

            migrationBuilder.DropTable(
                name: "Inquiries");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineryOption_Machinery_MachineryId",
                table: "MachineryOption",
                column: "MachineryId",
                principalTable: "Machinery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineryOption_Option_OptionId",
                table: "MachineryOption",
                column: "OptionId",
                principalTable: "Option",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
