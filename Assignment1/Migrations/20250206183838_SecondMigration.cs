using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPrice",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Products");
        }
    }
}
