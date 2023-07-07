using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompoundPlus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_CompoundCondition_CompoundConditionId",
                table: "Conditions");

            migrationBuilder.DropTable(
                name: "CompoundCondition");

            migrationBuilder.RenameColumn(
                name: "CompoundConditionId",
                table: "Conditions",
                newName: "ParentConditionId");

            migrationBuilder.RenameIndex(
                name: "IX_Conditions_CompoundConditionId",
                table: "Conditions",
                newName: "IX_Conditions_ParentConditionId");

            migrationBuilder.AddColumn<string>(
                name: "LogicalOperator",
                table: "Conditions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions",
                column: "ParentConditionId",
                principalTable: "Conditions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions");

            migrationBuilder.DropColumn(
                name: "LogicalOperator",
                table: "Conditions");

            migrationBuilder.RenameColumn(
                name: "ParentConditionId",
                table: "Conditions",
                newName: "CompoundConditionId");

            migrationBuilder.RenameIndex(
                name: "IX_Conditions_ParentConditionId",
                table: "Conditions",
                newName: "IX_Conditions_CompoundConditionId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_CompoundCondition_CompoundConditionId",
                table: "Conditions",
                column: "CompoundConditionId",
                principalTable: "CompoundCondition",
                principalColumn: "Id");
        }
    }
}
