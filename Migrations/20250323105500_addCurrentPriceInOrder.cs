using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPrn222.Migrations
{
    /// <inheritdoc />
    public partial class addCurrentPriceInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "OrderDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetails");
        }
    }
}
