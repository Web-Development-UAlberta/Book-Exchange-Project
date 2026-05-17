using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingListingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    google_place_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_addresses_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "listings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    isbn = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    condition = table.Column<int>(type: "book_condition", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    weight_grams = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "listing_status", nullable: false, defaultValueSql: "'active'::listing_status"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listings", x => x.id);
                    table.CheckConstraint("ck_listings_isbn", "isbn ~ '^[0-9]{13}$' OR isbn ~ '^[0-9X]{10}$'");
                    table.CheckConstraint("ck_listings_price", "price >= 0");
                    table.CheckConstraint("ck_listings_weight_grams", "weight_grams > 0");
                    table.ForeignKey(
                        name: "FK_listings_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_genres",
                schema: "public",
                columns: table => new
                {
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    genre_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listing_genres", x => new { x.listing_id, x.genre_id });
                    table.ForeignKey(
                        name: "FK_listing_genres_genres_genre_id",
                        column: x => x.genre_id,
                        principalSchema: "public",
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_listing_genres_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "public",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_user_id",
                schema: "public",
                table: "addresses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_genres_name",
                schema: "public",
                table: "genres",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_genres_genre_id",
                schema: "public",
                table: "listing_genres",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_isbn",
                schema: "public",
                table: "listings",
                column: "isbn");

            migrationBuilder.CreateIndex(
                name: "ix_listings_status",
                schema: "public",
                table: "listings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_listings_user_id",
                schema: "public",
                table: "listings",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "listing_genres",
                schema: "public");

            migrationBuilder.DropTable(
                name: "genres",
                schema: "public");

            migrationBuilder.DropTable(
                name: "listings",
                schema: "public");
        }
    }
}
