using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodFirst.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddListingReasonToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentOverride",
                table: "StoreInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StoreInventories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReasonNotes",
                table: "StoreInventories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnsellableSubReason",
                table: "StoreInventories",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreparationStartedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreparedByUserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollectionRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollectorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionRuns_Users_CollectorUserId",
                        column: x => x.CollectorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectionRuns_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorePickups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollectionRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderInRun = table.Column<int>(type: "int", nullable: false),
                    ArrivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TemperatureAtPickup = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    StoreSignatureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePickups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorePickups_CollectionRuns_CollectionRunId",
                        column: x => x.CollectionRunId,
                        principalTable: "CollectionRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorePickups_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorePickupItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorePickupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreInventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpectedQuantity = table.Column<int>(type: "int", nullable: false),
                    CollectedQuantity = table.Column<int>(type: "int", nullable: false),
                    IsConform = table.Column<bool>(type: "bit", nullable: false),
                    NonConformityReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePickupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorePickupItems_StoreInventories_StoreInventoryId",
                        column: x => x.StoreInventoryId,
                        principalTable: "StoreInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorePickupItems_StorePickups_StorePickupId",
                        column: x => x.StorePickupId,
                        principalTable: "StorePickups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PreparedByUserId",
                table: "Orders",
                column: "PreparedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionRuns_CollectorUserId",
                table: "CollectionRuns",
                column: "CollectorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionRuns_ZoneId",
                table: "CollectionRuns",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePickupItems_StoreInventoryId",
                table: "StorePickupItems",
                column: "StoreInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePickupItems_StorePickupId",
                table: "StorePickupItems",
                column: "StorePickupId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePickups_CollectionRunId",
                table: "StorePickups",
                column: "CollectionRunId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePickups_StoreId",
                table: "StorePickups",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_PreparedByUserId",
                table: "Orders",
                column: "PreparedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_PreparedByUserId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "StorePickupItems");

            migrationBuilder.DropTable(
                name: "StorePickups");

            migrationBuilder.DropTable(
                name: "CollectionRuns");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PreparedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountPercentOverride",
                table: "StoreInventories");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StoreInventories");

            migrationBuilder.DropColumn(
                name: "ReasonNotes",
                table: "StoreInventories");

            migrationBuilder.DropColumn(
                name: "UnsellableSubReason",
                table: "StoreInventories");

            migrationBuilder.DropColumn(
                name: "PreparationStartedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PreparedByUserId",
                table: "Orders");
        }
    }
}
