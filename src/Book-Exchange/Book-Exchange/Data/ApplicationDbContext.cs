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
        }
    }
}
