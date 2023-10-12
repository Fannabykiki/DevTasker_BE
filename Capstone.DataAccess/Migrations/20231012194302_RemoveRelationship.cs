using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class RemoveRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_PermissionSchemas_PermissionSchemaSchemaId",
                table: "Permissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PermissionSchemaSchemaId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_PermissionSchemas_PermissionSchemaSchemaId",
                table: "Permissions",
                column: "PermissionSchemaSchemaId",
                principalTable: "PermissionSchemas",
                principalColumn: "SchemaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_PermissionSchemas_PermissionSchemaSchemaId",
                table: "Permissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PermissionSchemaSchemaId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_PermissionSchemas_PermissionSchemaSchemaId",
                table: "Permissions",
                column: "PermissionSchemaSchemaId",
                principalTable: "PermissionSchemas",
                principalColumn: "SchemaId");
        }
    }
}
