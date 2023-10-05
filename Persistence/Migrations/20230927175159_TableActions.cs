using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TableActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "RowId",
                table: "Actions",
                newName: "TableId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_RowId",
                table: "Actions",
                newName: "IX_Actions_TableId");

            migrationBuilder.CreateTable(
                name: "ActionValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DecisionRowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionValues_DecisionRows_DecisionRowId",
                        column: x => x.DecisionRowId,
                        principalTable: "DecisionRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionValues_DecisionRowId",
                table: "ActionValues",
                column: "DecisionRowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionTables_TableId",
                table: "Actions",
                column: "TableId",
                principalTable: "DecisionTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionTables_TableId",
                table: "Actions");

            migrationBuilder.DropTable(
                name: "ActionValues");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "Actions",
                newName: "RowId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_TableId",
                table: "Actions",
                newName: "IX_Actions_RowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionRows_RowId",
                table: "Actions",
                column: "RowId",
                principalTable: "DecisionRows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
