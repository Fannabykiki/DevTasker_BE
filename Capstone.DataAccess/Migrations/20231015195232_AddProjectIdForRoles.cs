using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddProjectIdForRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ProjectId",
                table: "Roles",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Projects_ProjectId",
                table: "Roles",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Projects_ProjectId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_ProjectId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Roles");
        }
    }
}
