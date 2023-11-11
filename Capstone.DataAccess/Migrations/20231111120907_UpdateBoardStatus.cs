using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class UpdateBoardStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Status_StatusId",
                table: "Boards");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Status_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropTable(
                name: "TaskStatus");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoardStatus",
                columns: table => new
                {
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardStatus", x => x.BoardId);
                    table.ForeignKey(
                        name: "FK_BoardStatus_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "BoardId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_StatusId",
                table: "TaskHistories",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Status_StatusId",
                table: "Boards",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Status_StatusId",
                table: "TaskHistories",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Status_StatusId",
                table: "Boards");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Status_StatusId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "BoardStatus");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_StatusId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TaskHistories");

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => new { x.TaskId, x.StatusId });
                    table.ForeignKey(
                        name: "FK_TaskStatus_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_TaskStatus_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatus_StatusId",
                table: "TaskStatus",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Status_StatusId",
                table: "Boards",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Status_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId",
                principalTable: "Status",
                principalColumn: "StatusId");
        }
    }
}
