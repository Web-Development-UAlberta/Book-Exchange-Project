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

        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<ListingGenre> ListingGenres => Set<ListingGenre>();
        public DbSet<WishlistItem> Wishlist => Set<WishlistItem>();
        public DbSet<ExchangeRequest> ExchangeRequests => Set<ExchangeRequest>();
        public DbSet<ExchangeRequestItem> ExchangeRequestItems => Set<ExchangeRequestItem>();




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
            // Address
            builder.Entity<Address>(entity =>
            {

                entity.ToTable("addresses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.GooglePlaceId)
                    .HasColumnName("google_place_id")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Addresses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("ix_addresses_user_id");
            });

            // Genre
            builder.Entity<Genre>(entity =>
            {
                entity.ToTable("genres");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("ux_genres_name");

            });

            // Listing
            builder.Entity<Listing>(entity =>
            {
                entity.ToTable("listings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Isbn)
                    .HasColumnName("isbn")
                    .HasMaxLength(13)
                    .IsRequired();

                entity.Property(e => e.Condition)
                    .HasColumnName("condition")
                    .HasColumnType("book_condition")
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.WeightGrams)
                    .HasColumnName("weight_grams")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("listing_status")
                    .HasDefaultValueSql("'active'::listing_status")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Listings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("ix_listings_user_id");

                entity.HasIndex(e => e.Isbn)
                    .HasDatabaseName("ix_listings_isbn");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("ix_listings_status");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_listings_isbn", "isbn ~ '^[0-9]{13}$' OR isbn ~ '^[0-9X]{10}$'");
                    t.HasCheckConstraint("ck_listings_price", "price >= 0");
                    t.HasCheckConstraint("ck_listings_weight_grams", "weight_grams > 0");

                });
            });

            // ListingGenre
            builder.Entity<ListingGenre>(entity =>
            {
                entity.ToTable("listing_genres");
                entity.HasKey(e => new { e.ListingId, e.GenreId });
                entity.Property(e => e.ListingId).HasColumnName("listing_id");
                entity.Property(e => e.GenreId).HasColumnName("genre_id");

                entity.HasOne(e => e.Listing)
                    .WithMany(e => e.ListingGenres)
                    .HasForeignKey(e => e.ListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Genre)
                    .WithMany(e => e.ListingGenres)
                    .HasForeignKey(e => e.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.GenreId)
                    .HasDatabaseName("ix_listing_genres_genre_id");
            });

            // WishlistItem
            builder.Entity<WishlistItem>(entity =>
            {
                entity.ToTable("wishlist");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Isbn)
                    .HasColumnName("isbn")
                    .HasMaxLength(13)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.WishlistItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("ix_wishlist_user_id");

                entity.HasIndex(e => e.Isbn)
                    .HasDatabaseName("ix_wishlist_isbn");

                entity.HasIndex(e => new { e.UserId, e.Isbn })
                    .IsUnique()
                    .HasDatabaseName("ux_wishlist_user_id_isbn");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_wishlist_isbn", "isbn ~ '^[0-9]{13}$' OR isbn ~ '^[0-9X]{10}$'");
                });
            });

            // ExchangeRequest
            builder.Entity<ExchangeRequest>(entity =>
            {
                entity.ToTable("exchange_requests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TargetListingId)
                    .HasColumnName("target_listing_id")
                    .IsRequired();

                entity.Property(e => e.RequesterId)
                    .HasColumnName("requester_id")
                    .IsRequired();

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("exchange_type")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("exchange_status")
                    .HasDefaultValueSql("'requested'::exchange_status")
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasPrecision(10, 2);

                entity.Property(e => e.CounterOffer)
                    .HasColumnName("counter_offer")
                    .HasPrecision(10, 2);

                entity.Property(e => e.Message)
                    .HasColumnName("message");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(e => e.AcceptedAt).HasColumnName("accepted_at");

                entity.Property(e => e.CompletedAt).HasColumnName("completed_at");

                entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");

                entity.HasOne(e => e.TargetListing)
                    .WithMany(e => e.TargetExchangeRequests)
                    .HasForeignKey(e => e.TargetListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Requester)
                    .WithMany(e => e.ExchangeRequests)
                    .HasForeignKey(e => e.RequesterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TargetListingId)
                    .HasDatabaseName("ix_exchange_requests_target_listing_id");

                entity.HasIndex(e => e.RequesterId)
                    .HasDatabaseName("ix_exchange_requests_requester_id");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("ix_exchange_requests_status");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_exchange_requests_price", "price IS NULL OR price >= 0");
                    t.HasCheckConstraint("ck_exchange_requests_counter_offer", "counter_offer IS NULL OR counter_offer >= 0");
                });
            });

            // ExchangeRequestItem
            builder.Entity<ExchangeRequestItem>(entity =>
            {
                entity.ToTable("exchange_request_items");

                entity.HasKey(e => new { e.ExchangeRequestId, e.OfferedListingId });

                entity.Property(e => e.ExchangeRequestId)
                    .HasColumnName("exchange_request_id");
                entity.Property(e => e.OfferedListingId)
                    .HasColumnName("offered_listing_id");
                entity.HasOne(e => e.ExchangeRequest)
                    .WithMany(e => e.ExchangeRequestItems)
                    .HasForeignKey(e => e.ExchangeRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.OfferedListing)
                    .WithMany(e => e.OfferedInExchangeRequestItems)
                    .HasForeignKey(e => e.OfferedListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OfferedListingId)
                    .HasDatabaseName("ix_exchange_request_items_offered_listing_id");
            });
        }
    }
}
