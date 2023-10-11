using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class RemoveField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Boards_BoardId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BoardId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Boards_ProjectId",
                table: "Projects",
                column: "ProjectId",
                principalTable: "Boards",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Boards_ProjectId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "BoardId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BoardId",
                table: "Projects",
                column: "BoardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Boards_BoardId",
                table: "Projects",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
