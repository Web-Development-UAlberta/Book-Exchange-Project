using Book_Exchange.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)

        {
            base.OnModelCreating(builder);
            
            builder.HasDefaultSchema("public");

            // PostgreSQL enum mapping
            builder.HasPostgresEnum<BookCondition>("public", "book_condition");
            builder.HasPostgresEnum<ListingType>("public", "listing_type");
            builder.HasPostgresEnum<TransactionType>("public", "transaction_type");
            builder.HasPostgresEnum<TransactionStatus>("public", "transaction_status");
            builder.HasPostgresEnum<ShippingStatus>("public", "shipping_status");
            builder.HasPostgresEnum<NotificationType>("public", "notification_type");
            builder.HasPostgresEnum<NotificationStatus>("public", "notification_status");
            builder.HasPostgresEnum<MessageType>("public", "message_type");
            builder.HasPostgresEnum<MessageStatus>("public", "message_status");
            builder.HasPostgresEnum<LocalityType>("public", "locality_type");
        }
    }
}
