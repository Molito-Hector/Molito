using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SubProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentPropertyId",
                table: "RuleProperties",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties",
                column: "ParentPropertyId",
                principalTable: "RuleProperties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentPropertyId",
                table: "RuleProperties",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties",
                column: "ParentPropertyId",
                principalTable: "RuleProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
