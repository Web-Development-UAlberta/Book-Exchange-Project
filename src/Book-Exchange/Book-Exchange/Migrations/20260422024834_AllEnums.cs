using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AllEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .Annotation("Npgsql:Enum:public.listing_type", "sell,buy,swap")
                .Annotation("Npgsql:Enum:public.locality_type", "local,provincial,national,international")
                .Annotation("Npgsql:Enum:public.message_status", "sent,read")
                .Annotation("Npgsql:Enum:public.message_type", "text,offer")
                .Annotation("Npgsql:Enum:public.notification_status", "unread,read,archived")
                .Annotation("Npgsql:Enum:public.notification_type", "match_found,wishlist_available,new_message,offer_received,offer_accepted,offer_rejected,transaction_update")
                .Annotation("Npgsql:Enum:public.shipping_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:public.transaction_status", "proposed,negotiating,confirmed,shipped,completed,cancelled,disputed")
                .Annotation("Npgsql:Enum:public.transaction_type", "buy_sell,swap,multi_swap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "public",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "public",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "public",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "public",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "public",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "public",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "public",
                newName: "AspNetRoleClaims");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .OldAnnotation("Npgsql:Enum:public.listing_type", "sell,buy,swap")
                .OldAnnotation("Npgsql:Enum:public.locality_type", "local,provincial,national,international")
                .OldAnnotation("Npgsql:Enum:public.message_status", "sent,read")
                .OldAnnotation("Npgsql:Enum:public.message_type", "text,offer")
                .OldAnnotation("Npgsql:Enum:public.notification_status", "unread,read,archived")
                .OldAnnotation("Npgsql:Enum:public.notification_type", "match_found,wishlist_available,new_message,offer_received,offer_accepted,offer_rejected,transaction_update")
                .OldAnnotation("Npgsql:Enum:public.shipping_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:public.transaction_status", "proposed,negotiating,confirmed,shipped,completed,cancelled,disputed")
                .OldAnnotation("Npgsql:Enum:public.transaction_type", "buy_sell,swap,multi_swap");
        }
    }
}
