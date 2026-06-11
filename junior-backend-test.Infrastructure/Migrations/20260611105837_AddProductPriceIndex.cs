using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace junior_backend_test.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPriceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_products_price",
                table: "Products",
                column: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_products_price",
                table: "Products");
        }
    }
}
