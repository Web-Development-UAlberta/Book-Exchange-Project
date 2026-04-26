using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Transaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    buyer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: true),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_value = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.CheckConstraint("CK_transactions_buyer_or_seller", "\"buyer_id\" IS NOT NULL OR \"seller_id\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_transactions_AspNetUsers_buyer_id",
                        column: x => x.buyer_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactions_AspNetUsers_seller_id",
                        column: x => x.seller_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactions_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "transaction_listings",
                schema: "public",
                columns: table => new
                {
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_listings", x => new { x.transaction_id, x.listing_id });
                    table.ForeignKey(
                        name: "FK_transaction_listings_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_listings_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_listings_listing_id",
                schema: "public",
                table: "transaction_listings",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_buyer_id",
                schema: "public",
                table: "transactions",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_listing_id",
                schema: "public",
                table: "transactions",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_seller_id",
                schema: "public",
                table: "transactions",
                column: "seller_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction_listings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "public");
        }
    }
}
