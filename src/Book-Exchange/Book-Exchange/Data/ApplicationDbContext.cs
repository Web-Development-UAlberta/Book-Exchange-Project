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
            
            builder.HasDefaultSchema("public");

            builder.Entity<ApplicationUser>().ToTable("AspNetUsers", "public");
            builder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles", "public");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles", "public");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims", "public");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins", "public");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims", "public");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens", "public");

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
