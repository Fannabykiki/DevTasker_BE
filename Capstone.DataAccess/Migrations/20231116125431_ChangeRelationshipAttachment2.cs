using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class ChangeRelationshipAttachment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TaskComments_TaskCommentCommentId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_TaskCommentCommentId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "TaskCommentCommentId",
                table: "Attachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskCommentCommentId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TaskCommentCommentId",
                table: "Attachments",
                column: "TaskCommentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TaskComments_TaskCommentCommentId",
                table: "Attachments",
                column: "TaskCommentCommentId",
                principalTable: "TaskComments",
                principalColumn: "CommentId");
        }
    }
}
