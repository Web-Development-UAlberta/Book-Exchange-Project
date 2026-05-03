using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingListingExchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listing_genres",
                schema: "public");

            migrationBuilder.DropTable(
                name: "genres",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "ix_listings_status",
                schema: "public",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "public",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "counter_offer",
                schema: "public",
                table: "exchange_requests");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "public",
                table: "exchange_requests");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .Annotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .Annotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed")
                .OldAnnotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .OldAnnotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:public.exchange_type", "buy_sell,book_swap,book_swap_with_cash")
                .OldAnnotation("Npgsql:Enum:public.listing_status", "active,pending,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .OldAnnotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .Annotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:public.exchange_type", "buy_sell,book_swap,book_swap_with_cash")
                .Annotation("Npgsql:Enum:public.listing_status", "active,pending,completed,cancelled")
                .Annotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .Annotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed")
                .OldAnnotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .OldAnnotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .OldAnnotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed");

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "public",
                table: "listings",
                type: "listing_status",
                nullable: false,
                defaultValueSql: "'active'::listing_status");

            migrationBuilder.AddColumn<decimal>(
                name: "counter_offer",
                schema: "public",
                table: "exchange_requests",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                schema: "public",
                table: "exchange_requests",
                type: "exchange_type",
                nullable: false,
                defaultValue: 0);

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
                name: "ix_listings_status",
                schema: "public",
                table: "listings",
                column: "status");

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
        }
    }
}
