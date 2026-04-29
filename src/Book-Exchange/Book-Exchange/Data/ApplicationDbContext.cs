using Book_Exchange.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid
        >(options)
    {     
        protected override void OnModelCreating(ModelBuilder builder)

        {
            base.OnModelCreating(builder);
            // ASP.NET Identity table names
            builder.HasDefaultSchema("public");
            builder.Entity<ApplicationUser>().ToTable("asp_net_users", "public");
            builder.Entity<IdentityRole<Guid>>().ToTable("asp_net_roles", "public");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("asp_net_user_roles", "public");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("asp_net_user_claims", "public");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("asp_net_user_logins", "public");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("asp_net_role_claims", "public");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("asp_net_user_tokens", "public");
            // PostgreSQL enum types
            builder.HasPostgresEnum<BookCondition>("public", "book_condition");
            builder.HasPostgresEnum<ListingStatus>("public", "listing_status");
            builder.HasPostgresEnum<ExchangeType>("public", "exchange_type");
            builder.HasPostgresEnum<ExchangeStatus>("public", "exchange_status");
            builder.HasPostgresEnum<TransactionStatus>("public", "transaction_status");
            builder.HasPostgresEnum<ShipmentStatus>("public", "shipment_status");
            builder.HasPostgresEnum<NotificationStatus>("public", "notification_status");
            builder.HasPostgresEnum<NotificationCategory>("public", "notification_category");
            builder.HasPostgresEnum<MessageStatus>("public", "message_status");

        }
    }
}
