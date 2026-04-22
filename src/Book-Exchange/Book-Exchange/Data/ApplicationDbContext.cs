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
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<LocationDistance> LocationDistances => Set<LocationDistance>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<BookGenre> BookGenres => Set<BookGenre>();
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<WishlistItem> Wishlist => Set<WishlistItem>();
        public DbSet<Carrier> Carriers => Set<Carrier>();
        public DbSet<CarrierRate> CarrierRates => Set<CarrierRate>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionListing> TransactionListings => Set<TransactionListing>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<Review> Reviews => Set<Review>();


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

            // locations
            builder.Entity<Location>()
                .HasIndex(x => new { x.City, x.ProvinceState, x.Country })
                .IsUnique();

            // location_distances
            builder.Entity<LocationDistance>()
                .HasKey(x => new { x.FromLocationId, x.ToLocationId });

            builder.Entity<LocationDistance>()
                .HasOne(x => x.FromLocation)
                .WithMany(x => x.DistancesFrom)
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LocationDistance>()
                .HasOne(x => x.ToLocation)
                .WithMany(x => x.DistancesTo)
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LocationDistance>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_location_distances_not_same", "\"from_location_id\" <> \"to_location_id\"");
                    t.HasCheckConstraint("CK_location_distances_distance_nonnegative", "\"distance_km\" >= 0");

                });

            // addresses
            builder.Entity<Address>()
                .HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Address>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Address>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            // books
            builder.Entity<Book>()
                .HasIndex(x => x.Isbn13)
                .IsUnique();

            builder.Entity<Book>()
                .HasIndex(x => x.Isbn10)
                .IsUnique();

            builder.Entity<Book>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            // authors
            builder.Entity<Author>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // book_authors
            builder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            builder.Entity<BookAuthor>()
                .HasOne(x => x.Book)
                .WithMany(x => x.BookAuthors)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookAuthor>()
                .HasOne(x => x.Author)
                .WithMany(x => x.BookAuthors)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // genres
            builder.Entity<Genre>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // book_genres
            builder.Entity<BookGenre>()
                .HasKey(x => new { x.BookId, x.GenreId });

            builder.Entity<BookGenre>()
                .HasOne(x => x.Book)
                .WithMany(x => x.BookGenres)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookGenre>()
                .HasOne(x => x.Genre)
                .WithMany(x => x.BookGenres)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // listings
            builder.Entity<Listing>()
                .HasOne(x => x.User)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Listing>()
                .HasOne(x => x.Book)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            // wishlist
            builder.Entity<WishlistItem>()
                .HasIndex(x => new { x.UserId, x.BookId })
                .IsUnique();

            builder.Entity<WishlistItem>()
                .HasOne(x => x.User)
                .WithMany(x => x.WishlistItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishlistItem>()
                .HasOne(x => x.Book)
                .WithMany(x => x.WishlistItems)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // carriers

            builder.Entity<Carrier>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // carrier_rates
            builder.Entity<CarrierRate>()
                .HasOne(x => x.Carrier)
                .WithMany(x => x.Rates)
                .HasForeignKey(x => x.CarrierId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarrierRate>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            // transactions
            builder.Entity<Transaction>()
                .HasOne(x => x.Buyer)
                .WithMany(x => x.BuyerTransactions)
                .HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(x => x.Seller)
                .WithMany(x => x.SellerTransactions)
                .HasForeignKey(x => x.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(x => x.Listing)
                .WithMany(x => x.PrimaryTransactions)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Transaction>()
                .Property(x => x.Status)
                .HasDefaultValue(TransactionStatus.Proposed);

            builder.Entity<Transaction>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            builder.Entity<Transaction>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_transactions_buyer_or_seller", "\"buyer_id\" IS NOT NULL OR \"seller_id\" IS NOT NULL");
                });

            // transaction_listings
            builder.Entity<TransactionListing>()
                .HasKey(x => new { x.TransactionId, x.ListingId });

            builder.Entity<TransactionListing>()
                .HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionListings)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransactionListing>()
                .HasOne(x => x.Listing)
                .WithMany(x => x.TransactionListings)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // shipments
            builder.Entity<Shipment>()
                .HasOne(x => x.Transaction)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shipment>()
                .HasOne(x => x.SenderAddress)
                .WithMany(x => x.SenderShipments)
                .HasForeignKey(x => x.SenderAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Shipment>()
                .HasOne(x => x.ReceiverAddress)
                .WithMany(x => x.ReceiverShipments)
                .HasForeignKey(x => x.ReceiverAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Shipment>()
                .HasOne(x => x.Carrier)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.CarrierId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Shipment>()
                .Property(x => x.Status)
                .HasDefaultValue(ShippingStatus.Pending);

            builder.Entity<Shipment>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            // reviews
            builder.Entity<Review>()
                .HasIndex(x => new { x.TransactionId, x.ReviewerId, x.RevieweeId })
                .IsUnique();

            builder.Entity<Review>()
                .HasOne(x => x.Transaction)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(x => x.Reviewer)
                .WithMany(x => x.ReviewsWritten)
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(x => x.Reviewee)
                .WithMany(x => x.ReviewsReceived)
                .HasForeignKey(x => x.RevieweeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            builder.Entity<Review>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_reviews_rating_range", "\"rating\" BETWEEN 1 AND 5");
                    t.HasCheckConstraint("CK_reviews_reviewer_not_reviewee", "\"reviewer_id\" <> \"reviewee_id\"");
                });
        }
    }
}
