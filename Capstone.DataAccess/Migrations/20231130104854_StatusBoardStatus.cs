using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class StatusBoardStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "BoardStatus",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BoardStatus_StatusId",
                table: "BoardStatus",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardStatus_Status_StatusId",
                table: "BoardStatus",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardStatus_Status_StatusId",
                table: "BoardStatus");

            migrationBuilder.DropIndex(
                name: "IX_BoardStatus_StatusId",
                table: "BoardStatus");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "BoardStatus");
        }
    }
}
