using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanTrangSuc.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantitySold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantitySold",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantitySold",
                table: "Products");
        }
    }
}
