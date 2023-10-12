using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class FixConstrain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_PermissionSchemas_SchemaId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_SchemaId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "SchemaId",
                table: "Roles");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "PermissionSchemas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_PermissionSchemas_RoleId",
                table: "Roles",
                column: "RoleId",
                principalTable: "PermissionSchemas",
                principalColumn: "SchemaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_PermissionSchemas_RoleId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "PermissionSchemas");

            migrationBuilder.AddColumn<Guid>(
                name: "SchemaId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Roles_SchemaId",
                table: "Roles",
                column: "SchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_PermissionSchemas_SchemaId",
                table: "Roles",
                column: "SchemaId",
                principalTable: "PermissionSchemas",
                principalColumn: "SchemaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
