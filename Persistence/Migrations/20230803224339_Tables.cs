using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionRows_DecisionRowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_DecisionRows_DecisionRowId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_DecisionTables_DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.DropIndex(
                name: "IX_RuleProperties_DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.DropColumn(
                name: "DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.RenameColumn(
                name: "DecisionRowId",
                table: "Conditions",
                newName: "TableId");

            migrationBuilder.RenameIndex(
                name: "IX_Conditions_DecisionRowId",
                table: "Conditions",
                newName: "IX_Conditions_TableId");

            migrationBuilder.RenameColumn(
                name: "DecisionRowId",
                table: "Actions",
                newName: "RowId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_DecisionRowId",
                table: "Actions",
                newName: "IX_Actions_RowId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RuleId",
                table: "Conditions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConditionId",
                table: "Actions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "ConditionValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConditionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DecisionRowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionValues_DecisionRows_DecisionRowId",
                        column: x => x.DecisionRowId,
                        principalTable: "DecisionRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConditionValues_DecisionRowId",
                table: "ConditionValues",
                column: "DecisionRowId");

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
                name: "FK_Conditions_DecisionTables_TableId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions");

            migrationBuilder.DropTable(
                name: "ConditionValues");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "Conditions",
                newName: "DecisionRowId");

            migrationBuilder.RenameIndex(
                name: "IX_Conditions_TableId",
                table: "Conditions",
                newName: "IX_Conditions_DecisionRowId");

            migrationBuilder.RenameColumn(
                name: "RowId",
                table: "Actions",
                newName: "DecisionRowId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_RowId",
                table: "Actions",
                newName: "IX_Actions_DecisionRowId");

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionTableId",
                table: "RuleProperties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RuleId",
                table: "Conditions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConditionId",
                table: "Actions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RuleProperties_DecisionTableId",
                table: "RuleProperties",
                column: "DecisionTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionRows_DecisionRowId",
                table: "Actions",
                column: "DecisionRowId",
                principalTable: "DecisionRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_DecisionRows_DecisionRowId",
                table: "Conditions",
                column: "DecisionRowId",
                principalTable: "DecisionRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Rules_RuleId",
                table: "Conditions",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_DecisionTables_DecisionTableId",
                table: "RuleProperties",
                column: "DecisionTableId",
                principalTable: "DecisionTables",
                principalColumn: "Id");
        }
    }
}
