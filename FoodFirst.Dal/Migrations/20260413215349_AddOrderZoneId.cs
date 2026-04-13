using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodFirst.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderZoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ZoneId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ZoneId",
                table: "Orders",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Zones_ZoneId",
                table: "Orders",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Zones_ZoneId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ZoneId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Orders");
        }
    }
}
