using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPrn222.Migrations
{
    /// <inheritdoc />
    public partial class deletePamentMethodTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodNavigationPaymentMethodId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodNavigationPaymentMethodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodNavigationPaymentMethodId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodNavigationPaymentMethodId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodNavigationPaymentMethodId",
                table: "Orders",
                column: "PaymentMethodNavigationPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodNavigationPaymentMethodId",
                table: "Orders",
                column: "PaymentMethodNavigationPaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
