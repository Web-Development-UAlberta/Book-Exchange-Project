using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Shipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shipments",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_address_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_address_id = table.Column<Guid>(type: "uuid", nullable: false),
                    carrier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locality = table.Column<int>(type: "integer", nullable: true),
                    package_weight_kg = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    shipping_cost = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    label_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipments", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipments_addresses_receiver_address_id",
                        column: x => x.receiver_address_id,
                        principalSchema: "public",
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shipments_addresses_sender_address_id",
                        column: x => x.sender_address_id,
                        principalSchema: "public",
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shipments_carriers_carrier_id",
                        column: x => x.carrier_id,
                        principalSchema: "public",
                        principalTable: "carriers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_shipments_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shipments_carrier_id",
                schema: "public",
                table: "shipments",
                column: "carrier_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_receiver_address_id",
                schema: "public",
                table: "shipments",
                column: "receiver_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_sender_address_id",
                schema: "public",
                table: "shipments",
                column: "sender_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_transaction_id",
                schema: "public",
                table: "shipments",
                column: "transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shipments",
                schema: "public");
        }
    }
}
