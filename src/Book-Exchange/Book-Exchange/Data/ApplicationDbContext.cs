using Book_Exchange.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Book_Exchange.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid
        >(options)
    {

        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<WishlistItem> Wishlist => Set<WishlistItem>();
        public DbSet<ExchangeRequest> ExchangeRequests => Set<ExchangeRequest>();
        public DbSet<ExchangeRequestItem> ExchangeRequestItems => Set<ExchangeRequestItem>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionStatusHistory> TransactionStatusHistories { get; set; }
        public DbSet<Carrier> Carriers => Set<Carrier>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Message> Messages => Set<Message>();

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
            builder.HasPostgresEnum<ExchangeStatus>("public", "exchange_status");
            builder.HasPostgresEnum<TransactionStatus>("public", "transaction_status");
            builder.HasPostgresEnum<ShipmentStatus>("public", "shipment_status");
            builder.HasPostgresEnum<NotificationCategory>("public", "notification_category");
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


                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_listings_isbn", "isbn ~ '^[0-9]{13}$' OR isbn ~ '^[0-9X]{10}$'");
                    t.HasCheckConstraint("ck_listings_price", "price >= 0");
                    t.HasCheckConstraint("ck_listings_weight_grams", "weight_grams > 0");

                });
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

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("exchange_status")
                    .HasDefaultValueSql("'requested'::exchange_status")
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasColumnName("price")
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

            // Transaction
            builder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ExchangeRequestId)
                    .HasColumnName("exchange_request_id")
                    .IsRequired();                

                entity.Property(e => e.TotalValue)
                    .HasColumnName("total_value")
                    .HasPrecision(10, 2);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");

                entity.Property(e => e.CompletedAt).HasColumnName("completed_at");

                entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");

                entity.HasOne(e => e.ExchangeRequest)
                    .WithOne(e => e.Transaction)
                    .HasForeignKey<Transaction>(e => e.ExchangeRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ExchangeRequestId)
                    .IsUnique()
                    .HasDatabaseName("ux_transactions_exchange_request_id");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_transactions_total_value", "total_value IS NULL OR total_value >= 0");
                });
            });

            builder.Entity<TransactionStatusHistory>(entity =>
            {
                entity.ToTable("transaction_status_history");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
                entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Transaction)
                .WithMany(e => e.StatusHistory)
                .HasForeignKey(e => e.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UpdatedByUser)
                .WithMany(e => e.TransactionStatusUpdatedByUser)
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("transaction_status")
                    .HasDefaultValueSql("'confirmed'::transaction_status")
                    .IsRequired();
            });

            // Carrier
            builder.Entity<Carrier>(entity =>
            {
                entity.ToTable("carriers");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.BaseCost)
                    .HasColumnName("base_cost")
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.CostPerKg)
                    .HasColumnName("cost_per_kg")
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.CostPerKm)
                    .HasColumnName("cost_per_km")
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.MaxWeightGrams)
                    .HasColumnName("max_weight_grams");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("ux_carriers_name");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_carriers_base_cost", "base_cost >= 0");
                    t.HasCheckConstraint("ck_carriers_cost_per_kg", "cost_per_kg >= 0");
                    t.HasCheckConstraint("ck_carriers_cost_per_km", "cost_per_km >= 0");
                    t.HasCheckConstraint("ck_carriers_max_weight_grams", "max_weight_grams IS NULL OR max_weight_grams > 0");
                });
            });

            // Shipment
            builder.Entity<Shipment>(entity =>
            {
                entity.ToTable("shipments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .IsRequired();

                entity.Property(e => e.SenderAddressId)
                    .HasColumnName("sender_address_id")
                    .IsRequired();

                entity.Property(e => e.ReceiverAddressId)
                    .HasColumnName("receiver_address_id")
                    .IsRequired();

                entity.Property(e => e.CarrierId)
                    .HasColumnName("carrier_id");

                entity.Property(e => e.PackageWeightGrams)
                    .HasColumnName("package_weight_grams")
                    .IsRequired();

                entity.Property(e => e.DistanceKm)
                    .HasColumnName("distance_km")
                    .HasPrecision(10, 2);

                entity.Property(e => e.ShippingCost)
                    .HasColumnName("shipping_cost")
                    .HasPrecision(10, 2);

                entity.Property(e => e.TrackingNumber)
                    .HasColumnName("tracking_number")
                    .HasMaxLength(100);

                entity.Property(e => e.LabelUrl)
                    .HasColumnName("label_url");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("shipment_status")
                    .HasDefaultValueSql("'pending'::shipment_status")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(e => e.Transaction)
                    .WithMany(e => e.Shipments)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SenderAddress)
                    .WithMany(e => e.SenderShipments)
                    .HasForeignKey(e => e.SenderAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReceiverAddress)
                    .WithMany(e => e.ReceiverShipments)
                    .HasForeignKey(e => e.ReceiverAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Carrier)
                    .WithMany(e => e.Shipments)
                    .HasForeignKey(e => e.CarrierId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.TransactionId)
                    .HasDatabaseName("ix_shipments_transaction_id");

                entity.HasIndex(e => e.SenderAddressId)
                    .HasDatabaseName("ix_shipments_sender_address_id");

                entity.HasIndex(e => e.ReceiverAddressId)
                    .HasDatabaseName("ix_shipments_receiver_address_id");

                entity.HasIndex(e => e.CarrierId)
                    .HasDatabaseName("ix_shipments_carrier_id");

                entity.ToTable(t =>

                {
                    t.HasCheckConstraint("ck_shipments_package_weight_grams", "package_weight_grams > 0");
                    t.HasCheckConstraint("ck_shipments_distance_km", "distance_km IS NULL OR distance_km >= 0");
                    t.HasCheckConstraint("ck_shipments_shipping_cost", "shipping_cost IS NULL OR shipping_cost >= 0");
                });
            });

            // Review
            builder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .IsRequired();

                entity.Property(e => e.ReviewerId)
                    .HasColumnName("reviewer_id")
                    .IsRequired();

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .IsRequired();

                entity.Property(e => e.Comment)
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(e => e.Transaction)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Reviewer)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.ReviewerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TransactionId)
                    .HasDatabaseName("ix_reviews_transaction_id");

                entity.HasIndex(e => e.ReviewerId)
                    .HasDatabaseName("ix_reviews_reviewer_id");

                entity.HasIndex(e => new { e.TransactionId, e.ReviewerId })
                    .IsUnique()
                    .HasDatabaseName("ux_reviews_transaction_id_reviewer_id");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_reviews_rating", "rating BETWEEN 1 AND 5");
                });
            });

            // Notification
            builder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasColumnType("notification_category")
                    .IsRequired();

                entity.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .IsRequired();

                entity.Property(e => e.RelatedListingId)
                    .HasColumnName("related_listing_id");

                entity.Property(e => e.RelatedExchangeRequestId)
                    .HasColumnName("related_exchange_request_id");

                entity.Property(e => e.RelatedTransactionId)
                    .HasColumnName("related_transaction_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(e => e.ReadAt)
                    .HasColumnName("read_at");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RelatedListing)
                    .WithMany(e => e.Notifications)
                    .HasForeignKey(e => e.RelatedListingId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.RelatedExchangeRequest)
                    .WithMany(e => e.Notifications)
                    .HasForeignKey(e => e.RelatedExchangeRequestId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.RelatedTransaction)
                    .WithMany(e => e.Notifications)
                    .HasForeignKey(e => e.RelatedTransactionId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("ix_notifications_user_id");

                entity.HasIndex(e => e.IsRead)
                    .HasDatabaseName("ix_notifications_is_read");

                entity.HasIndex(e => e.RelatedListingId)
                    .HasDatabaseName("ix_notifications_related_listing_id");

                entity.HasIndex(e => e.RelatedExchangeRequestId)
                    .HasDatabaseName("ix_notifications_related_exchange_request_id");

                entity.HasIndex(e => e.RelatedTransactionId)
                    .HasDatabaseName("ix_notifications_related_transaction_id");
            });

            // Message
            builder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SenderId)
                    .HasColumnName("sender_id")
                    .IsRequired();

                entity.Property(e => e.ReceiverId)
                    .HasColumnName("receiver_id")
                    .IsRequired();

                entity.Property(e => e.MessageText)
                    .HasColumnName("message_text");

                entity.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.ListingId)
                    .HasColumnName("listing_id");

                entity.Property(e => e.ExchangeRequestId)
                    .HasColumnName("exchange_request_id");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(e => e.Sender)
                    .WithMany(e => e.SentMessages)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Receiver)
                    .WithMany(e => e.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Listing)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.ListingId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ExchangeRequest)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.ExchangeRequestId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Transaction)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.SenderId)
                    .HasDatabaseName("ix_messages_sender_id");

                entity.HasIndex(e => e.ReceiverId)
                    .HasDatabaseName("ix_messages_receiver_id");

                entity.HasIndex(e => e.ListingId)
                    .HasDatabaseName("ix_messages_listing_id");

                entity.HasIndex(e => e.ExchangeRequestId)
                    .HasDatabaseName("ix_messages_exchange_request_id");

                entity.HasIndex(e => e.TransactionId)
                    .HasDatabaseName("ix_messages_transaction_id");

            });
        }
    }
}
