using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoreCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_DecisionTables_TableId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions",
                column: "RowId",
                principalTable: "DecisionRows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions",
                column: "ParentConditionId",
                principalTable: "Conditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_DecisionTables_TableId",
                table: "Conditions",
                column: "TableId",
                principalTable: "DecisionTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_DecisionTables_TableId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions",
                column: "RowId",
                principalTable: "DecisionRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Conditions_ParentConditionId",
                table: "Conditions",
                column: "ParentConditionId",
                principalTable: "Conditions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_DecisionTables_TableId",
                table: "Conditions",
                column: "TableId",
                principalTable: "DecisionTables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id");
        }
    }
}
