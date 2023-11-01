using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddFKforTicketTypeAndTickets2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTypes_TicketId",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TypeId",
                table: "Tickets",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTypes_TypeId",
                table: "Tickets",
                column: "TypeId",
                principalTable: "TicketTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTypes_TypeId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TypeId",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTypes_TicketId",
                table: "Tickets",
                column: "TicketId",
                principalTable: "TicketTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
