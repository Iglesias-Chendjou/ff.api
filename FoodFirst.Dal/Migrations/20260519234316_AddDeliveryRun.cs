using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodFirst.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryRunId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderInRun",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeliveryRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DeliveryPersonUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryRuns_Users_DeliveryPersonUserId",
                        column: x => x.DeliveryPersonUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryRuns_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryRunId",
                table: "Deliveries",
                column: "DeliveryRunId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRuns_Code",
                table: "DeliveryRuns",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRuns_DeliveryPersonUserId",
                table: "DeliveryRuns",
                column: "DeliveryPersonUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRuns_ZoneId",
                table: "DeliveryRuns",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryRuns_DeliveryRunId",
                table: "Deliveries",
                column: "DeliveryRunId",
                principalTable: "DeliveryRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryRuns_DeliveryRunId",
                table: "Deliveries");

            migrationBuilder.DropTable(
                name: "DeliveryRuns");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryRunId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryRunId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "OrderInRun",
                table: "Deliveries");
        }
    }
}
