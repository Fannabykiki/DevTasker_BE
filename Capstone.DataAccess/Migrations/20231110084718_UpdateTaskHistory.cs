using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class UpdateTaskHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusTaskHistory");

            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "TaskHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "ChangeBy",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStatusId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousStatusId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Projects",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_ChangeBy",
                table: "TaskHistories",
                column: "ChangeBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_ProjectMembers_ChangeBy",
                table: "TaskHistories",
                column: "ChangeBy",
                principalTable: "ProjectMembers",
                principalColumn: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Status_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId",
                principalTable: "Status",
                principalColumn: "StatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_ProjectMembers_ChangeBy",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Status_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_ChangeBy",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "ChangeBy",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "PreviousStatusId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStatus",
                table: "TaskHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousStatus",
                table: "TaskHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StatusTaskHistory",
                columns: table => new
                {
                    TaskHistoriesHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskStatusStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTaskHistory", x => new { x.TaskHistoriesHistoryId, x.TaskStatusStatusId });
                    table.ForeignKey(
                        name: "FK_StatusTaskHistory_Status_TaskStatusStatusId",
                        column: x => x.TaskStatusStatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusTaskHistory_TaskHistories_TaskHistoriesHistoryId",
                        column: x => x.TaskHistoriesHistoryId,
                        principalTable: "TaskHistories",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusTaskHistory_TaskStatusStatusId",
                table: "StatusTaskHistory",
                column: "TaskStatusStatusId");
        }
    }
}
