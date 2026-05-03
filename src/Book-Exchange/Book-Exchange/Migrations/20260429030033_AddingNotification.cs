using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<int>(type: "notification_category", nullable: false),
                    status = table.Column<int>(type: "notification_status", nullable: false, defaultValueSql: "'unread'::notification_status"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    related_listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_exchange_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_exchange_requests_related_exchange_request_id",
                        column: x => x.related_exchange_request_id,
                        principalSchema: "public",
                        principalTable: "exchange_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notifications_listings_related_listing_id",
                        column: x => x.related_listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notifications_transactions_related_transaction_id",
                        column: x => x.related_transaction_id,
                        principalSchema: "public",
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_related_exchange_request_id",
                schema: "public",
                table: "notifications",
                column: "related_exchange_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_related_listing_id",
                schema: "public",
                table: "notifications",
                column: "related_listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_related_transaction_id",
                schema: "public",
                table: "notifications",
                column: "related_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_status",
                schema: "public",
                table: "notifications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                schema: "public",
                table: "notifications",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications",
                schema: "public");
        }
    }
}
