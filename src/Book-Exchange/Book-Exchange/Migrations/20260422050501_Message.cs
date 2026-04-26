using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Message : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "messages",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_text = table.Column<string>(type: "text", nullable: true),
                    message_type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    offer_amount = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_AspNetUsers_receiver_id",
                        column: x => x.receiver_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_AspNetUsers_sender_id",
                        column: x => x.sender_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_messages_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messages_listing_id",
                schema: "public",
                table: "messages",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_receiver_id",
                schema: "public",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                schema: "public",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_transaction_id",
                schema: "public",
                table: "messages",
                column: "transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages",
                schema: "public");
        }
    }
}
