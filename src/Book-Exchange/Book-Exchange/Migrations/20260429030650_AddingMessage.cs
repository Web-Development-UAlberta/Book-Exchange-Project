using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingMessage : Migration
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
                    status = table.Column<int>(type: "message_status", nullable: false, defaultValueSql: "'sent'::message_status"),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    exchange_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_asp_net_users_receiver_id",
                        column: x => x.receiver_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_asp_net_users_sender_id",
                        column: x => x.sender_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_exchange_requests_exchange_request_id",
                        column: x => x.exchange_request_id,
                        principalSchema: "public",
                        principalTable: "exchange_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "ix_messages_exchange_request_id",
                schema: "public",
                table: "messages",
                column: "exchange_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_listing_id",
                schema: "public",
                table: "messages",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_receiver_id",
                schema: "public",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id",
                schema: "public",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_transaction_id",
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
