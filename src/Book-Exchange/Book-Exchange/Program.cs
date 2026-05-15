using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<BookCondition>("book_condition");
dataSourceBuilder.MapEnum<ExchangeStatus>("exchange_status");
dataSourceBuilder.MapEnum<TransactionStatus>("transaction_status");
dataSourceBuilder.MapEnum<ShipmentStatus>("shipment_status");
dataSourceBuilder.MapEnum<NotificationCategory>("notification_category");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(dataSource, o =>
    {
        o.MapEnum<BookCondition>("book_condition");
        o.MapEnum<ExchangeStatus>("exchange_status");
        o.MapEnum<TransactionStatus>("transaction_status");
        o.MapEnum<ShipmentStatus>("shipment_status");
        o.MapEnum<NotificationCategory>("notification_category");
    });
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IListingService, ListingService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IExchangeRequestService, ExchangeRequestService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddHttpClient<IPlaceApiService, GooglePlaceApiService>();
builder.Services.AddHttpClient<IBookSearchApi, GoogleBookSearchApi>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Required for GitHub Actions UI test runs
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var testUser = await SeedUserAsync(userManager, "test@test.com", "Test1234!", new Guid("aaaa0001-0000-0000-0000-000000000001"));
    var otherUser = await SeedUserAsync(userManager, "otheruser@test.com", "Test1234!", new Guid("aaaa0002-0000-0000-0000-000000000002"));
    var noReviewUser = await SeedUserAsync(userManager, "noreview@test.com", "Test1234!", new Guid("aaaa0003-0000-0000-0000-000000000003"));

    await SeedAddressesAsync(db, testUser.Id, otherUser.Id);
    await SeedCarriersAsync(db);
    await SeedListingsAndWishlistsAsync(db, testUser.Id, otherUser.Id);
    await SeedExchangeRequestsTransactionsAndShipmentsAsync(db, testUser.Id, otherUser.Id);
    await SeedMessagesAsync(db, testUser.Id, otherUser.Id);
    await SeedNotificationsAsync(db, testUser.Id, otherUser.Id);
    await SeedCompletedTransactionDataAsync(db, testUser.Id, otherUser.Id);
    await SeedReviewsAsync(db, testUser.Id, otherUser.Id);
}
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseStaticFiles();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();

// Required for GitHub Actions UI test runs
static async Task<ApplicationUser> SeedUserAsync(
    UserManager<ApplicationUser> userManager,
    string email,
    string password,
    Guid? pinnedID = null)
{
    var existing = await userManager.FindByEmailAsync(email);
    if (existing != null) return existing;

    var user = new ApplicationUser
    {
        Id = pinnedID ?? Guid.NewGuid(),
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };

    await userManager.CreateAsync(user, password);
    return user;
}

static async Task SeedAddressesAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var address1Id = new Guid("aaaaaaaa-0001-0000-0000-000000000001");
    var address2Id = new Guid("aaaaaaaa-0002-0000-0000-000000000002");

    if (await db.Addresses.AnyAsync(a =>
            a.Id == address1Id ||
            a.Id == address2Id))
    {
        return;
    }

    db.Addresses.AddRange(
        new Address
        {
            Id = address1Id,
            UserId = testUserId,
            FullName = "Test User Edmonton Address",
            GooglePlaceId = "ChIJI__egEUioFMRXRX2SgygH0E",
            IsDefault = true,
            CreatedAt = new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc)
        },
        new Address
        {
            Id = address2Id,
            UserId = otherUserId,
            FullName = "Other User Calgary Address",
            GooglePlaceId = "ChIJ1T-EnwNwcVMROrZStrE7bSY",
            IsDefault = true,
            CreatedAt = new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc)
        }
    );

    await db.SaveChangesAsync();
}

static async Task SeedCarriersAsync(ApplicationDbContext db)
{
    var carrier1 = new Guid("bbbbbbbb-0001-0000-0000-000000000001");
    if (await db.Carriers.AnyAsync(c => c.Id == carrier1)) return;

    db.Carriers.AddRange(
        new Carrier
        {
            Id = carrier1,
            Name = "Canada Post",
            BaseCost = 9.99m,
            CostPerKg = 2.25m,
            CostPerKm = 0.035m,
            MaxWeightGrams = 30000,
            IsActive = true
        },
        new Carrier
        {
            Id = new Guid("bbbbbbbb-0002-0000-0000-000000000002"),
            Name = "Purolator",
            BaseCost = 12.99m,
            CostPerKg = 2.75m,
            CostPerKm = 0.045m,
            MaxWeightGrams = 32000,
            IsActive = true
        },
        new Carrier
        {
            Id = new Guid("bbbbbbbb-0003-0000-0000-000000000003"),
            Name = "FedEx Canada",
            BaseCost = 14.99m,
            CostPerKg = 3.00m,
            CostPerKm = 0.050m,
            MaxWeightGrams = 30000,
            IsActive = true
        },
        new Carrier
        {
            Id = new Guid("bbbbbbbb-0004-0000-0000-000000000004"),
            Name = "UPS Canada",
            BaseCost = 13.49m,
            CostPerKg = 2.95m,
            CostPerKm = 0.048m,
            MaxWeightGrams = 30000,
            IsActive = true
        }
    );

    await db.SaveChangesAsync();
}

static async Task SeedListingsAndWishlistsAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var listing1 = new Guid("cccccccc-0001-0000-0000-000000000001");
    if (await db.Listings.AnyAsync(l => l.Id == listing1)) return;

    db.Listings.AddRange(
        new Listing { Id = listing1, UserId = testUserId, Isbn = "9780141439600", Condition = BookCondition.Good, Price = 10.00m, WeightGrams = 320 },
        new Listing { Id = new Guid("cccccccc-0002-0000-0000-000000000002"), UserId = testUserId, Isbn = "9780141439518", Condition = BookCondition.VeryGood, Price = 12.50m, WeightGrams = 350 },
        new Listing { Id = new Guid("cccccccc-0003-0000-0000-000000000003"), UserId = testUserId, Isbn = "9780140449136", Condition = BookCondition.LikeNew, Price = 15.00m, WeightGrams = 420 },
        new Listing { Id = new Guid("cccccccc-0004-0000-0000-000000000004"), UserId = testUserId, Isbn = "9780061120084", Condition = BookCondition.Good, Price = 11.00m, WeightGrams = 300 },

        new Listing { Id = new Guid("cccccccc-0005-0000-0000-000000000005"), UserId = otherUserId, Isbn = "9780439064873", Condition = BookCondition.VeryGood, Price = 9.50m, WeightGrams = 310 },
        new Listing { Id = new Guid("cccccccc-0006-0000-0000-000000000006"), UserId = otherUserId, Isbn = "9780743273565", Condition = BookCondition.Good, Price = 13.00m, WeightGrams = 340 },
        new Listing { Id = new Guid("cccccccc-0007-0000-0000-000000000007"), UserId = otherUserId, Isbn = "9780307474278", Condition = BookCondition.Acceptable, Price = 8.00m, WeightGrams = 360 },
        new Listing { Id = new Guid("cccccccc-0008-0000-0000-000000000008"), UserId = otherUserId, Isbn = "9780385472579", Condition = BookCondition.LikeNew, Price = 14.00m, WeightGrams = 390 }
    );

    db.Wishlist.AddRange(
        new WishlistItem { Id = new Guid("dddddddd-0001-0000-0000-000000000001"), UserId = testUserId, Isbn = "9780439064873", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0002-0000-0000-000000000002"), UserId = testUserId, Isbn = "9780743273565", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0003-0000-0000-000000000003"), UserId = testUserId, Isbn = "9780307474278", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0004-0000-0000-000000000004"), UserId = testUserId, Isbn = "9780385472579", IsActive = true },

        new WishlistItem { Id = new Guid("dddddddd-0005-0000-0000-000000000005"), UserId = otherUserId, Isbn = "9780141439600", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0006-0000-0000-000000000006"), UserId = otherUserId, Isbn = "9780141439518", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0007-0000-0000-000000000007"), UserId = otherUserId, Isbn = "9780140449136", IsActive = true },
        new WishlistItem { Id = new Guid("dddddddd-0008-0000-0000-000000000008"), UserId = otherUserId, Isbn = "9780061120084", IsActive = true }
    );

    await db.SaveChangesAsync();
}

static async Task SeedExchangeRequestsTransactionsAndShipmentsAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var request1 = new Guid("eeeeeeee-1001-0000-0000-000000000001");
    if (await db.ExchangeRequests.AnyAsync(e => e.Id == request1)) return;

    var now = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

    db.ExchangeRequests.AddRange(
        new ExchangeRequest
        {
            Id = request1,
            TargetListingId = new Guid("cccccccc-0005-0000-0000-000000000005"),
            RequesterId = testUserId,
            Status = ExchangeStatus.Accepted,
            Price = 0,
            Message = "I can offer Pride and Prejudice for your Harry Potter book.",
            CreatedAt = now.AddHours(9),
            AcceptedAt = now.AddHours(10)
        },
        new ExchangeRequest
        {
            Id = new Guid("eeeeeeee-1002-0000-0000-000000000002"),
            TargetListingId = new Guid("cccccccc-0006-0000-0000-000000000006"),
            RequesterId = testUserId,
            Status = ExchangeStatus.Requested,
            Price = 3.00m,
            Message = "I am interested in this book and can add a small cash top-up.",
            CreatedAt = now.AddHours(11)
        },
        new ExchangeRequest
        {
            Id = new Guid("eeeeeeee-1003-0000-0000-000000000003"),
            TargetListingId = new Guid("cccccccc-0001-0000-0000-000000000001"),
            RequesterId = otherUserId,
            Status = ExchangeStatus.Accepted,
            Price = 0,
            Message = "I can offer Harry Potter for your A Tale of Two Cities.",
            CreatedAt = now.AddHours(12),
            AcceptedAt = now.AddHours(13)
        },
        new ExchangeRequest
        {
            Id = new Guid("eeeeeeee-1004-0000-0000-000000000004"),
            TargetListingId = new Guid("cccccccc-0002-0000-0000-000000000002"),
            RequesterId = otherUserId,
            Status = ExchangeStatus.Requested,
            Price = 2.00m,
            Message = "I would like to request this listing.",
            CreatedAt = now.AddHours(14)
        }
    );

    db.ExchangeRequestItems.AddRange(
        new ExchangeRequestItem
        {
            ExchangeRequestId = request1,
            OfferedListingId = new Guid("cccccccc-0001-0000-0000-000000000001")
        },
        new ExchangeRequestItem
        {
            ExchangeRequestId = new Guid("eeeeeeee-1002-0000-0000-000000000002"),
            OfferedListingId = new Guid("cccccccc-0002-0000-0000-000000000002")
        },
        new ExchangeRequestItem
        {
            ExchangeRequestId = new Guid("eeeeeeee-1003-0000-0000-000000000003"),
            OfferedListingId = new Guid("cccccccc-0005-0000-0000-000000000005")
        },
        new ExchangeRequestItem
        {
            ExchangeRequestId = new Guid("eeeeeeee-1004-0000-0000-000000000004"),
            OfferedListingId = new Guid("cccccccc-0006-0000-0000-000000000006")
        }
    );

    db.Transactions.AddRange(
        new Transaction
        {
            Id = new Guid("99999999-0001-0000-0000-000000000001"),
            ExchangeRequestId = request1,
            TotalValue = 19.50m,
            CreatedAt = now.AddHours(10),
            ConfirmedAt = now.AddHours(10).AddMinutes(10)
        },
        new Transaction
        {
            Id = new Guid("99999999-0002-0000-0000-000000000002"),
            ExchangeRequestId = new Guid("eeeeeeee-1003-0000-0000-000000000003"),
            TotalValue = 19.50m,
            CreatedAt = now.AddHours(13),
            ConfirmedAt = now.AddHours(13).AddMinutes(10)
        }
    );

    db.Shipments.AddRange(
        new Shipment
        {
            Id = new Guid("88888888-0001-0000-0000-000000000001"),
            TransactionId = new Guid("99999999-0001-0000-0000-000000000001"),
            SenderAddressId = new Guid("aaaaaaaa-0001-0000-0000-000000000001"),
            ReceiverAddressId = new Guid("aaaaaaaa-0002-0000-0000-000000000002"),
            CarrierId = new Guid("bbbbbbbb-0001-0000-0000-000000000001"),
            PackageWeightGrams = 320,
            DistanceKm = 300.00m,
            ShippingCost = 21.21m,
            TrackingNumber = "CPMOCK000001",
            LabelUrl = "https://example.com/mock-labels/CPMOCK000001.pdf",
            Status = ShipmentStatus.Shipped,
            CreatedAt = now.AddDays(1)
        },
        new Shipment
        {
            Id = new Guid("88888888-0002-0000-0000-000000000002"),
            TransactionId = new Guid("99999999-0002-0000-0000-000000000002"),
            SenderAddressId = new Guid("aaaaaaaa-0002-0000-0000-000000000002"),
            ReceiverAddressId = new Guid("aaaaaaaa-0001-0000-0000-000000000001"),
            CarrierId = new Guid("bbbbbbbb-0002-0000-0000-000000000002"),
            PackageWeightGrams = 310,
            DistanceKm = 300.00m,
            ShippingCost = 27.34m,
            TrackingNumber = "PUROMOCK000001",
            LabelUrl = "https://example.com/mock-labels/PUROMOCK000001.pdf",
            Status = ShipmentStatus.LabelCreated,
            CreatedAt = now.AddDays(1).AddHours(2)
        }
    );

    await db.SaveChangesAsync();
}

static async Task SeedMessagesAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var msg1 = new Guid("eeeeeeee-0001-0000-0000-000000000001");
    if (await db.Messages.AnyAsync(m => m.Id == msg1)) return;

    var now = new DateTime(2026, 5, 01, 0, 0, 0, DateTimeKind.Utc);

    db.Messages.AddRange(
        new Message
        {
            Id = msg1,
            SenderId = testUserId,
            ReceiverId = otherUserId,
            MessageText = "Hi! Is your copy of A Tale of Two Cities still available?",
            IsRead = false,
            CreatedAt = now.AddHours(9)
        },
        new Message
        {
            Id = new Guid("eeeeeeee-0002-0000-0000-000000000002"),
            SenderId = otherUserId,
            ReceiverId = testUserId,
            MessageText = "Yes it is! Good condition, looking to swap for something by Jane Austen.",
            IsRead = true,
            CreatedAt = now.AddHours(9).AddMinutes(15)
        },
        new Message
        {
            Id = new Guid("eeeeeeee-0003-0000-0000-000000000003"),
            SenderId = testUserId,
            ReceiverId = otherUserId,
            MessageText = "I have Pride and Prejudice — interested?",
            IsRead = false,
            CreatedAt = now.AddHours(9).AddMinutes(30)
        },
        new Message
        {
            Id = new Guid("eeeeeeee-0004-0000-0000-000000000004"),
            SenderId = otherUserId,
            ReceiverId = testUserId,
            MessageText = "That works great for me. Let's set up the exchange!",
            IsRead = true,
            CreatedAt = now.AddHours(9).AddMinutes(45)
        },
        new Message
        {
            Id = new Guid("eeeeeeee-0005-0000-0000-000000000005"),
            SenderId = testUserId,
            ReceiverId = otherUserId,
            MessageText = "Perfect. I'll submit the exchange request now. Check your notifications!",
            IsRead = false,
            CreatedAt = now.AddHours(10)
        }
    );

    await db.SaveChangesAsync();
}

static async Task SeedNotificationsAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    if (db.Notifications.Any()) return;
    var notif1 = new Guid("ffffffff-0001-0000-0000-000000000001");
    if (await db.Notifications.AnyAsync(n => n.Id == notif1)) return;

    db.Notifications.AddRange(
        new Notification
        {
            Id = notif1,
            UserId = testUserId,
            Category = NotificationCategory.MatchFound,
            Title = "Book Match Found",
            Message = "A book from your wishlist is now available: \"Harry Potter and the Chamber of Secrets\"",
            IsRead = false,
            ReadAt = null,
            RelatedListingId = new Guid("cccccccc-0005-0000-0000-000000000005"),
            CreatedAt = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0002-0000-0000-000000000002"),
            UserId = otherUserId,
            Category = NotificationCategory.NewMessage,
            Title = "New Message",
            Message = "You have a new message from test@test.com.",
            IsRead = true,
            ReadAt = new DateTime(2026, 5, 2, 9, 16, 0, DateTimeKind.Utc),
            RelatedListingId = new Guid("cccccccc-0001-0000-0000-000000000001"),
            CreatedAt = new DateTime(2026, 5, 2, 9, 0, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0003-0000-0000-000000000003"),
            UserId = otherUserId,
            Category = NotificationCategory.ExchangeRequested,
            Title = "New Exchange Request",
            Message = "test@test.com has requested an exchange for \"A Tale of Two Cities\".",
            IsRead = false,
            ReadAt = null,
            RelatedListingId = new Guid("cccccccc-0001-0000-0000-000000000001"),
            RelatedExchangeRequestId = new Guid("eeeeeeee-1003-0000-0000-000000000003"),
            CreatedAt = new DateTime(2026, 5, 2, 10, 5, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0004-0000-0000-000000000004"),
            UserId = testUserId,
            Category = NotificationCategory.ExchangeAccepted,
            Title = "Exchange Accepted",
            Message = "Your exchange request for \"Harry Potter and the Chamber of Secrets\" was accepted.",
            IsRead = true,
            ReadAt = new DateTime(2026, 5, 3, 10, 30, 0, DateTimeKind.Utc),
            RelatedListingId = new Guid("cccccccc-0005-0000-0000-000000000005"),
            RelatedExchangeRequestId = new Guid("eeeeeeee-1001-0000-0000-000000000001"),
            CreatedAt = new DateTime(2026, 5, 3, 10, 20, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0005-0000-0000-000000000005"),
            UserId = testUserId,
            Category = NotificationCategory.TransactionUpdate,
            Title = "Transaction Created",
            Message = "A transaction has been created for \"Harry Potter and the Chamber of Secrets\". You can now arrange shipping.",
            IsRead = false,
            ReadAt = null,
            RelatedListingId = new Guid("cccccccc-0005-0000-0000-000000000005"),
            RelatedExchangeRequestId = new Guid("eeeeeeee-1001-0000-0000-000000000001"),
            RelatedTransactionId = new Guid("99999999-0001-0000-0000-000000000001"),
            CreatedAt = new DateTime(2026, 5, 3, 14, 0, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0006-0000-0000-000000000006"),
            UserId = otherUserId,
            Category = NotificationCategory.WishlistAvailable,
            Title = "Wishlist Book Available",
            Message = "A book from your wishlist is now available: \"A Tale of Two Cities\".",
            IsRead = false,
            ReadAt = null,
            RelatedListingId = new Guid("cccccccc-0001-0000-0000-000000000001"),
            CreatedAt = new DateTime(2026, 5, 4, 9, 0, 0, DateTimeKind.Utc)
        },
        new Notification
        {
            Id = new Guid("ffffffff-0007-0000-0000-000000000007"),
            UserId = testUserId,
            Category = NotificationCategory.ExchangeRejected,
            Title = "Exchange Rejected",
            Message = "Your exchange request for \"The Great Gatsby\" was declined.",
            IsRead = true,
            ReadAt = new DateTime(2026, 5, 4, 11, 0, 0, DateTimeKind.Utc),
            CreatedAt = new DateTime(2026, 5, 4, 10, 30, 0, DateTimeKind.Utc)
        }
    );

    await db.SaveChangesAsync();
}

static async Task SeedCompletedTransactionDataAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var completedAt = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    // ── Block 1: promote transactions 0001 and 0002 to Completed ────────────
    var guard1 = new Guid("77777777-0001-0000-0000-000000000001");
    if (!await db.TransactionStatusHistories.AnyAsync(h => h.Id == guard1))
    {
        db.TransactionStatusHistories.AddRange(
            new TransactionStatusHistory { Id = guard1, TransactionId = new Guid("99999999-0001-0000-0000-000000000001"), Status = TransactionStatus.Shipped, UpdatedByUserId = testUserId, UpdatedAt = completedAt.AddDays(-2) },
            new TransactionStatusHistory { Id = new Guid("77777777-0002-0000-0000-000000000002"), TransactionId = new Guid("99999999-0001-0000-0000-000000000001"), Status = TransactionStatus.Completed, UpdatedByUserId = otherUserId, UpdatedAt = completedAt }
        );
        var txn1 = await db.Transactions.FindAsync(new Guid("99999999-0001-0000-0000-000000000001"));
        if (txn1 != null) txn1.CompletedAt = completedAt;

        db.TransactionStatusHistories.AddRange(
            new TransactionStatusHistory { Id = new Guid("77777777-0003-0000-0000-000000000003"), TransactionId = new Guid("99999999-0002-0000-0000-000000000002"), Status = TransactionStatus.Shipped, UpdatedByUserId = otherUserId, UpdatedAt = completedAt.AddDays(-1) },
            new TransactionStatusHistory { Id = new Guid("77777777-0004-0000-0000-000000000004"), TransactionId = new Guid("99999999-0002-0000-0000-000000000002"), Status = TransactionStatus.Completed, UpdatedByUserId = testUserId, UpdatedAt = completedAt.AddHours(6) }
        );
        var txn2 = await db.Transactions.FindAsync(new Guid("99999999-0002-0000-0000-000000000002"));
        if (txn2 != null) txn2.CompletedAt = completedAt.AddHours(6);

        await db.SaveChangesAsync();
    }

    // ── Block 2: transaction 0003 (fresh, unreviewed) ────────────────────────
    var guard2 = new Guid("99999999-0003-0000-0000-000000000003");
    if (!await db.Transactions.AnyAsync(t => t.Id == guard2))
    {
        db.ExchangeRequests.Add(new ExchangeRequest { Id = new Guid("eeeeeeee-1005-0000-0000-000000000005"), TargetListingId = new Guid("cccccccc-0007-0000-0000-000000000007"), RequesterId = testUserId, Status = ExchangeStatus.Accepted, Price = 0, Message = "Fresh request seeded for UI-REVIEW-05.", CreatedAt = completedAt.AddDays(5), AcceptedAt = completedAt.AddDays(5).AddHours(1) });
        db.ExchangeRequestItems.Add(new ExchangeRequestItem { ExchangeRequestId = new Guid("eeeeeeee-1005-0000-0000-000000000005"), OfferedListingId = new Guid("cccccccc-0003-0000-0000-000000000003") });
        db.Transactions.Add(new Transaction { Id = guard2, ExchangeRequestId = new Guid("eeeeeeee-1005-0000-0000-000000000005"), TotalValue = 15.00m, CreatedAt = completedAt.AddDays(5).AddHours(1), ConfirmedAt = completedAt.AddDays(5).AddHours(1).AddMinutes(5), CompletedAt = completedAt.AddDays(7) });
        db.TransactionStatusHistories.AddRange(
            new TransactionStatusHistory { Id = new Guid("77777777-0005-0000-0000-000000000005"), TransactionId = guard2, Status = TransactionStatus.Shipped, UpdatedByUserId = testUserId, UpdatedAt = completedAt.AddDays(6) },
            new TransactionStatusHistory { Id = new Guid("77777777-0006-0000-0000-000000000006"), TransactionId = guard2, Status = TransactionStatus.Completed, UpdatedByUserId = otherUserId, UpdatedAt = completedAt.AddDays(7) }
        );
        await db.SaveChangesAsync();
    }

    // ── Block 3: transaction 0004 (incomplete, for UI-REVIEW-08) ────────────
    var guard3 = new Guid("99999999-0004-0000-0000-000000000004");
    if (!await db.Transactions.AnyAsync(t => t.Id == guard3))
    {
        db.ExchangeRequests.Add(new ExchangeRequest { Id = new Guid("eeeeeeee-1006-0000-0000-000000000006"), TargetListingId = new Guid("cccccccc-0008-0000-0000-000000000008"), RequesterId = testUserId, Status = ExchangeStatus.Accepted, Price = 0, Message = "Incomplete transaction seeded for UI-REVIEW-08.", CreatedAt = completedAt.AddDays(10), AcceptedAt = completedAt.AddDays(10).AddHours(1) });
        db.ExchangeRequestItems.Add(new ExchangeRequestItem { ExchangeRequestId = new Guid("eeeeeeee-1006-0000-0000-000000000006"), OfferedListingId = new Guid("cccccccc-0004-0000-0000-000000000004") });
        db.Transactions.Add(new Transaction { Id = guard3, ExchangeRequestId = new Guid("eeeeeeee-1006-0000-0000-000000000006"), TotalValue = 14.00m, CreatedAt = completedAt.AddDays(10).AddHours(1), ConfirmedAt = completedAt.AddDays(10).AddHours(1).AddMinutes(5) });
        db.TransactionStatusHistories.Add(new TransactionStatusHistory { Id = new Guid("77777777-0007-0000-0000-000000000007"), TransactionId = guard3, Status = TransactionStatus.Confirmed, UpdatedByUserId = testUserId, UpdatedAt = completedAt.AddDays(10).AddHours(1).AddMinutes(5) });
        await db.SaveChangesAsync();
    }
}

static async Task SeedReviewsAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var review1 = new Guid("11111111-0001-0000-0000-000000000001");
    if (await db.Reviews.AnyAsync(r => r.Id == review1)) return;

    db.Reviews.AddRange(
    new Review
    {
        Id = review1,
        TransactionId = new Guid("99999999-0001-0000-0000-000000000001"),
        ReviewerId = testUserId,
        Rating = 5,
        Comment = "Smooth exchange, book was exactly as described.",
        CreatedAt = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc)
    },
    new Review
    {
        Id = new Guid("11111111-0002-0000-0000-000000000002"),
        TransactionId = new Guid("99999999-0002-0000-0000-000000000002"),
        ReviewerId = otherUserId,
        Rating = 4,
        Comment = "Great swap partner, would exchange again.",
        CreatedAt = new DateTime(2026, 5, 2, 10, 0, 0, DateTimeKind.Utc)
    }
);

    await db.Reviews.AddRangeAsync();
    await db.SaveChangesAsync();
}
