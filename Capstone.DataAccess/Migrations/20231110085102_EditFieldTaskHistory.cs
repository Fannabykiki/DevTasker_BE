using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class EditFieldTaskHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Tasks_TicketId",
                table: "TaskHistories");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "TaskHistories",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskHistories_TicketId",
                table: "TaskHistories",
                newName: "IX_TaskHistories_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Tasks_TaskId",
                table: "TaskHistories",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Tasks_TaskId",
                table: "TaskHistories");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "TaskHistories",
                newName: "TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskHistories_TaskId",
                table: "TaskHistories",
                newName: "IX_TaskHistories_TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Tasks_TicketId",
                table: "TaskHistories",
                column: "TicketId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
