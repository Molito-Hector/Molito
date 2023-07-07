using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Compound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompoundConditionId",
                table: "Conditions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompoundCondition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Operator = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundCondition", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conditions_CompoundConditionId",
                table: "Conditions",
                column: "CompoundConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_CompoundCondition_CompoundConditionId",
                table: "Conditions",
                column: "CompoundConditionId",
                principalTable: "CompoundCondition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_CompoundCondition_CompoundConditionId",
                table: "Conditions");

            migrationBuilder.DropTable(
                name: "CompoundCondition");

            migrationBuilder.DropIndex(
                name: "IX_Conditions_CompoundConditionId",
                table: "Conditions");

            migrationBuilder.DropColumn(
                name: "CompoundConditionId",
                table: "Conditions");
        }
    }
}
