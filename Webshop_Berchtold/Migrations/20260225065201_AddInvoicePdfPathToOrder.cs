using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webshop_Berchtold.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicePdfPathToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoicePdfPath",
                table: "Orders",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoicePdfPath",
                table: "Orders");
        }
    }
}
