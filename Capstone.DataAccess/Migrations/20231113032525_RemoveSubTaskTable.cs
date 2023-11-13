using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class RemoveSubTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_SubTask_SubTaskId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_SubTask_CommentId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_SubTask_SubTaskId",
                table: "TaskHistories");

            migrationBuilder.DropTable(
                name: "SubTask");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_SubTaskId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_SubTaskId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "SubTaskId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "SubTaskId",
                table: "TaskComments");

            migrationBuilder.DropColumn(
                name: "SubTaskId",
                table: "Attachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubTaskId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SubTaskId",
                table: "TaskComments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubTaskId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubTask",
                columns: table => new
                {
                    SubTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignTo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Decription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTask", x => x.SubTaskId);
                    table.ForeignKey(
                        name: "FK_SubTask_BoardStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "BoardStatus",
                        principalColumn: "BoardStatusId");
                    table.ForeignKey(
                        name: "FK_SubTask_Interations_InterationId",
                        column: x => x.InterationId,
                        principalTable: "Interations",
                        principalColumn: "InterationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubTask_PriorityLevels_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "PriorityLevels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubTask_ProjectMembers_AssignTo",
                        column: x => x.AssignTo,
                        principalTable: "ProjectMembers",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubTask_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "SubTasks",
                        principalColumn: "TaskId");
                    table.ForeignKey(
                        name: "FK_SubTask_TaskTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_SubTaskId",
                table: "TaskHistories",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_SubTaskId",
                table: "Attachments",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_AssignTo",
                table: "SubTask",
                column: "AssignTo");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_InterationId",
                table: "SubTask",
                column: "InterationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_PriorityId",
                table: "SubTask",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_StatusId",
                table: "SubTask",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_TaskId",
                table: "SubTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_TypeId",
                table: "SubTask",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_SubTask_SubTaskId",
                table: "Attachments",
                column: "SubTaskId",
                principalTable: "SubTask",
                principalColumn: "SubTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_SubTask_CommentId",
                table: "TaskComments",
                column: "CommentId",
                principalTable: "SubTask",
                principalColumn: "SubTaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_SubTask_SubTaskId",
                table: "TaskHistories",
                column: "SubTaskId",
                principalTable: "SubTask",
                principalColumn: "SubTaskId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
