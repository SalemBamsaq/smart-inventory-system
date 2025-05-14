using Microsoft.EntityFrameworkCore.Migrations;
using SmartInventorySystem.Data;
#nullable disable

namespace SmartInventorySystem.Migrations
{
    public partial class FixSupplierContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Suppliers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
