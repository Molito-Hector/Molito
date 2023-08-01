using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActionCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Rules_RuleId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "RuleId",
                table: "Actions",
                newName: "ConditionId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_RuleId",
                table: "Actions",
                newName: "IX_Actions_ConditionId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rules",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Conditions_ConditionId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rules");

            migrationBuilder.RenameColumn(
                name: "ConditionId",
                table: "Actions",
                newName: "RuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_ConditionId",
                table: "Actions",
                newName: "IX_Actions_RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Rules_RuleId",
                table: "Actions",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
