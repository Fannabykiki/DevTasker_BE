using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class UpdateTaskStatusClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatus_Tasks_TaskId",
                table: "TaskStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus",
                columns: new[] { "TaskId", "StatusId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatus_Tasks_TaskId",
                table: "TaskStatus",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatus_Tasks_TaskId",
                table: "TaskStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatus_Tasks_TaskId",
                table: "TaskStatus",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
