using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class ChangeRelationshipMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Boards_BoardId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "Tickets",
                newName: "InterationId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_BoardId",
                table: "Tickets",
                newName: "IX_Tickets_InterationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Interations_InterationId",
                table: "Tickets",
                column: "InterationId",
                principalTable: "Interations",
                principalColumn: "InterationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Interations_InterationId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "InterationId",
                table: "Tickets",
                newName: "BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_InterationId",
                table: "Tickets",
                newName: "IX_Tickets_BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Boards_BoardId",
                table: "Tickets",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
