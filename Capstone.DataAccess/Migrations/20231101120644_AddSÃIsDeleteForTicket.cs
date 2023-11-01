using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddSÃIsDeleteForTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeleteTime",
                table: "Tickets",
                newName: "DeleteAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Tickets",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "DeleteAt",
                table: "Tickets",
                newName: "DeleteTime");
        }
    }
}
