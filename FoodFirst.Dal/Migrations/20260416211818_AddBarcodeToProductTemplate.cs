using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodFirst.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeToProductTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "ProductTemplates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "ProductTemplates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NutritionGrade",
                table: "ProductTemplates",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_Barcode",
                table: "ProductTemplates",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductTemplates_Barcode",
                table: "ProductTemplates");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "ProductTemplates");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "ProductTemplates");

            migrationBuilder.DropColumn(
                name: "NutritionGrade",
                table: "ProductTemplates");
        }
    }
}
