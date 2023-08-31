using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PropertyCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties",
                column: "ParentPropertyId",
                principalTable: "RuleProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_RuleProperties_ParentPropertyId",
                table: "RuleProperties",
                column: "ParentPropertyId",
                principalTable: "RuleProperties",
                principalColumn: "Id");
        }
    }
}
