using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingExchangeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exchange_requests",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "exchange_type", nullable: false),
                    status = table.Column<int>(type: "exchange_status", nullable: false, defaultValueSql: "'requested'::exchange_status"),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    counter_offer = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    accepted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_requests", x => x.id);
                    table.CheckConstraint("ck_exchange_requests_counter_offer", "counter_offer IS NULL OR counter_offer >= 0");
                    table.CheckConstraint("ck_exchange_requests_price", "price IS NULL OR price >= 0");
                    table.ForeignKey(
                        name: "FK_exchange_requests_asp_net_users_requester_id",
                        column: x => x.requester_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exchange_requests_listings_target_listing_id",
                        column: x => x.target_listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exchange_request_items",
                schema: "public",
                columns: table => new
                {
                    exchange_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offered_listing_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_request_items", x => new { x.exchange_request_id, x.offered_listing_id });
                    table.ForeignKey(
                        name: "FK_exchange_request_items_exchange_requests_exchange_request_id",
                        column: x => x.exchange_request_id,
                        principalSchema: "public",
                        principalTable: "exchange_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exchange_request_items_listings_offered_listing_id",
                        column: x => x.offered_listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exchange_request_items_offered_listing_id",
                schema: "public",
                table: "exchange_request_items",
                column: "offered_listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_exchange_requests_requester_id",
                schema: "public",
                table: "exchange_requests",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_exchange_requests_status",
                schema: "public",
                table: "exchange_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_exchange_requests_target_listing_id",
                schema: "public",
                table: "exchange_requests",
                column: "target_listing_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchange_request_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exchange_requests",
                schema: "public");
        }
    }
}
