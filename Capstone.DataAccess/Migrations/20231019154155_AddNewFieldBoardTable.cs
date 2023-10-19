using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddNewFieldBoardTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Boards",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Boards");
        }
    }
}
