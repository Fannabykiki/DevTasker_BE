using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class ChangeRelationshipAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TaskComments_CommentId",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Attachments",
                newName: "TaskCommentCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_CommentId",
                table: "Attachments",
                newName: "IX_Attachments_TaskCommentCommentId");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TaskId",
                table: "Attachments",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TaskComments_TaskCommentCommentId",
                table: "Attachments",
                column: "TaskCommentCommentId",
                principalTable: "TaskComments",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tasks_TaskId",
                table: "Attachments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TaskComments_TaskCommentCommentId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tasks_TaskId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_TaskId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "TaskCommentCommentId",
                table: "Attachments",
                newName: "CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_TaskCommentCommentId",
                table: "Attachments",
                newName: "IX_Attachments_CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TaskComments_CommentId",
                table: "Attachments",
                column: "CommentId",
                principalTable: "TaskComments",
                principalColumn: "CommentId");
        }
    }
}
