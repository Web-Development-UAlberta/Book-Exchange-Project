using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carriers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    base_cost = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    cost_per_kg = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    cost_per_km = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    max_weight_grams = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carriers", x => x.id);
                    table.CheckConstraint("ck_carriers_base_cost", "base_cost >= 0");
                    table.CheckConstraint("ck_carriers_cost_per_kg", "cost_per_kg >= 0");
                    table.CheckConstraint("ck_carriers_cost_per_km", "cost_per_km >= 0");
                    table.CheckConstraint("ck_carriers_max_weight_grams", "max_weight_grams IS NULL OR max_weight_grams > 0");
                });

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
                    package_weight_grams = table.Column<int>(type: "integer", nullable: false),
                    distance_km = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    shipping_cost = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    label_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "shipment_status", nullable: false, defaultValueSql: "'pending'::shipment_status"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipments", x => x.id);
                    table.CheckConstraint("ck_shipments_distance_km", "distance_km IS NULL OR distance_km >= 0");
                    table.CheckConstraint("ck_shipments_package_weight_grams", "package_weight_grams > 0");
                    table.CheckConstraint("ck_shipments_shipping_cost", "shipping_cost IS NULL OR shipping_cost >= 0");
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
                name: "ux_carriers_name",
                schema: "public",
                table: "carriers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipments_carrier_id",
                schema: "public",
                table: "shipments",
                column: "carrier_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_receiver_address_id",
                schema: "public",
                table: "shipments",
                column: "receiver_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_sender_address_id",
                schema: "public",
                table: "shipments",
                column: "sender_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_transaction_id",
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

            migrationBuilder.DropTable(
                name: "carriers",
                schema: "public");
        }
    }
}
