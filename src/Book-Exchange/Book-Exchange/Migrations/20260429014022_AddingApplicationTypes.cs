using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class AddingApplicationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .Annotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:public.exchange_type", "buy_sell,book_swap,book_swap_with_cash")
                .Annotation("Npgsql:Enum:public.listing_status", "active,pending,completed,cancelled")
                .Annotation("Npgsql:Enum:public.message_status", "sent,read")
                .Annotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .Annotation("Npgsql:Enum:public.notification_status", "unread,read,archived")
                .Annotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.book_condition", "like_new,very_good,good,acceptable,poor")
                .OldAnnotation("Npgsql:Enum:public.exchange_status", "requested,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:public.exchange_type", "buy_sell,book_swap,book_swap_with_cash")
                .OldAnnotation("Npgsql:Enum:public.listing_status", "active,pending,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:public.message_status", "sent,read")
                .OldAnnotation("Npgsql:Enum:public.notification_category", "match_found,wishlist_available,new_message,exchange_requested,exchange_accepted,exchange_rejected,transaction_update")
                .OldAnnotation("Npgsql:Enum:public.notification_status", "unread,read,archived")
                .OldAnnotation("Npgsql:Enum:public.shipment_status", "pending,quoted,label_created,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:public.transaction_status", "confirmed,shipped,completed,cancelled,disputed");
        }
    }
}
