using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class UpdateBoardStatusPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardStatus",
                table: "BoardStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardStatus",
                table: "BoardStatus",
                column: "BoardStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardStatus_BoardId",
                table: "BoardStatus",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardStatus",
                table: "BoardStatus");

            migrationBuilder.DropIndex(
                name: "IX_BoardStatus_BoardId",
                table: "BoardStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardStatus",
                table: "BoardStatus",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_BoardStatus_CurrentStatusId",
                table: "TaskHistories",
                column: "CurrentStatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_BoardStatus_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "BoardStatus",
                principalColumn: "BoardId");
        }
    }
}
