using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RuleProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_Rules_RuleId",
                table: "RuleProperties");

            migrationBuilder.RenameColumn(
                name: "RuleId",
                table: "RuleProperties",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_RuleProperties_RuleId",
                table: "RuleProperties",
                newName: "IX_RuleProperties_ProjectId");

            migrationBuilder.AddColumn<Guid>(
                name: "RuleProjectId",
                table: "Rules",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionTableId",
                table: "RuleProperties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionRowId",
                table: "Conditions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionTableId",
                table: "Comments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RuleProjectId",
                table: "Comments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionRowId",
                table: "Actions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RuleProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DecisionTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    RuleProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EvaluationType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DecisionTables_RuleProjects_RuleProjectId",
                        column: x => x.RuleProjectId,
                        principalTable: "RuleProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleProjectMembers",
                columns: table => new
                {
                    AppUserId = table.Column<string>(type: "TEXT", nullable: false),
                    RuleProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsOwner = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleProjectMembers", x => new { x.AppUserId, x.RuleProjectId });
                    table.ForeignKey(
                        name: "FK_RuleProjectMembers_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleProjectMembers_RuleProjects_RuleProjectId",
                        column: x => x.RuleProjectId,
                        principalTable: "RuleProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DecisionRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TableId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DecisionRows_DecisionTables_TableId",
                        column: x => x.TableId,
                        principalTable: "DecisionTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleProjectId",
                table: "Rules",
                column: "RuleProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleProperties_DecisionTableId",
                table: "RuleProperties",
                column: "DecisionTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Conditions_DecisionRowId",
                table: "Conditions",
                column: "DecisionRowId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DecisionTableId",
                table: "Comments",
                column: "DecisionTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RuleProjectId",
                table: "Comments",
                column: "RuleProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_DecisionRowId",
                table: "Actions",
                column: "DecisionRowId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionRows_TableId",
                table: "DecisionRows",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionTables_RuleProjectId",
                table: "DecisionTables",
                column: "RuleProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleProjectMembers_RuleProjectId",
                table: "RuleProjectMembers",
                column: "RuleProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_DecisionRows_DecisionRowId",
                table: "Actions",
                column: "DecisionRowId",
                principalTable: "DecisionRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DecisionTables_DecisionTableId",
                table: "Comments",
                column: "DecisionTableId",
                principalTable: "DecisionTables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_RuleProjects_RuleProjectId",
                table: "Comments",
                column: "RuleProjectId",
                principalTable: "RuleProjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_DecisionRows_DecisionRowId",
                table: "Conditions",
                column: "DecisionRowId",
                principalTable: "DecisionRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_DecisionTables_DecisionTableId",
                table: "RuleProperties",
                column: "DecisionTableId",
                principalTable: "DecisionTables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_RuleProjects_ProjectId",
                table: "RuleProperties",
                column: "ProjectId",
                principalTable: "RuleProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_RuleProjects_RuleProjectId",
                table: "Rules",
                column: "RuleProjectId",
                principalTable: "RuleProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_DecisionRows_DecisionRowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DecisionTables_DecisionTableId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_RuleProjects_RuleProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_DecisionRows_DecisionRowId",
                table: "Conditions");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_DecisionTables_DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleProperties_RuleProjects_ProjectId",
                table: "RuleProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_Rules_RuleProjects_RuleProjectId",
                table: "Rules");

            migrationBuilder.DropTable(
                name: "DecisionRows");

            migrationBuilder.DropTable(
                name: "RuleProjectMembers");

            migrationBuilder.DropTable(
                name: "DecisionTables");

            migrationBuilder.DropTable(
                name: "RuleProjects");

            migrationBuilder.DropIndex(
                name: "IX_Rules_RuleProjectId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_RuleProperties_DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.DropIndex(
                name: "IX_Conditions_DecisionRowId",
                table: "Conditions");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DecisionTableId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_RuleProjectId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Actions_DecisionRowId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "RuleProjectId",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "DecisionTableId",
                table: "RuleProperties");

            migrationBuilder.DropColumn(
                name: "DecisionRowId",
                table: "Conditions");

            migrationBuilder.DropColumn(
                name: "DecisionTableId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RuleProjectId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DecisionRowId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "RuleProperties",
                newName: "RuleId");

            migrationBuilder.RenameIndex(
                name: "IX_RuleProperties_ProjectId",
                table: "RuleProperties",
                newName: "IX_RuleProperties_RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleProperties_Rules_RuleId",
                table: "RuleProperties",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
