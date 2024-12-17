using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MachineryType_Machinery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machinery_MachineryTypes_MachineryTypeId",
                table: "Machinery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MachineryTypes",
                table: "MachineryTypes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Machinery");

            migrationBuilder.RenameTable(
                name: "MachineryTypes",
                newName: "MachineryType");

            migrationBuilder.AlterColumn<int>(
                name: "MachineryTypeId",
                table: "Machinery",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MachineryType",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineryType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MachineryType",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MachineryType",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MachineryType",
                table: "MachineryType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Machinery_MachineryType_MachineryTypeId",
                table: "Machinery",
                column: "MachineryTypeId",
                principalTable: "MachineryType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machinery_MachineryType_MachineryTypeId",
                table: "Machinery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MachineryType",
                table: "MachineryType");

            migrationBuilder.RenameTable(
                name: "MachineryType",
                newName: "MachineryTypes");

            migrationBuilder.AlterColumn<int>(
                name: "MachineryTypeId",
                table: "Machinery",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Machinery",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MachineryTypes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineryTypes",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MachineryTypes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MachineryTypes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MachineryTypes",
                table: "MachineryTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Machinery_MachineryTypes_MachineryTypeId",
                table: "Machinery",
                column: "MachineryTypeId",
                principalTable: "MachineryTypes",
                principalColumn: "Id");
        }
    }
}
