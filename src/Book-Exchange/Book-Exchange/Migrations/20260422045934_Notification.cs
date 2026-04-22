using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class Notification : Migration
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
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    related_listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_book_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_wishlist_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_books_related_book_id",
                        column: x => x.related_book_id,
                        principalSchema: "public",
                        principalTable: "books",
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
                    table.ForeignKey(
                        name: "FK_notifications_wishlist_related_wishlist_id",
                        column: x => x.related_wishlist_id,
                        principalSchema: "public",
                        principalTable: "wishlist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_book_id",
                schema: "public",
                table: "notifications",
                column: "related_book_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_listing_id",
                schema: "public",
                table: "notifications",
                column: "related_listing_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_transaction_id",
                schema: "public",
                table: "notifications",
                column: "related_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_wishlist_id",
                schema: "public",
                table: "notifications",
                column: "related_wishlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
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
