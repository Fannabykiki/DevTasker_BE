using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddExprietime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExprireTime",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExprireTime",
                table: "Schema",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExprireTime",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExprireTime",
                table: "Attachments",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExprireTime",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ExprireTime",
                table: "Schema");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ExprireTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ExprireTime",
                table: "Attachments");
        }
    }
}
