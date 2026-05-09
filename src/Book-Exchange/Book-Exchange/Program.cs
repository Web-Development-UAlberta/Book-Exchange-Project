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

    var testUser = await SeedUserAsync(userManager, "test@test.com", "Test1234!");
    var otherUser = await SeedUserAsync(userManager, "otheruser@test.com", "Test1234!");

    await SeedMessagesAsync(db, testUser.Id, otherUser.Id);
    await SeedNotificationsAsync(db, testUser.Id, otherUser.Id);
}
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();

// Required for GitHub Actions UI test runs
static async Task<ApplicationUser> SeedUserAsync(
    UserManager<ApplicationUser> userManager,
    string email,
    string password)
{
    var existing = await userManager.FindByEmailAsync(email);
    if (existing != null) return existing;

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };

    await userManager.CreateAsync(user, password);
    return user;
}

static async Task SeedMessagesAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var msg1 = new Guid("eeeeeeee-0001-0000-0000-000000000001");
    if (await db.Messages.AnyAsync(m => m.Id == msg1)) return;

    var now = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc);

    db.Messages.AddRange(
        new Message { Id = msg1, SenderId = testUserId, ReceiverId = otherUserId, MessageText = "Hi! Is your copy of A Tale of Two Cities still available?", IsRead = false, CreatedAt = now.AddHours(9) },
        new Message { Id = new Guid("eeeeeeee-0002-0000-0000-000000000002"), SenderId = otherUserId, ReceiverId = testUserId, MessageText = "Yes it is! Good condition, looking to swap for something by Jane Austen.", IsRead = true, CreatedAt = now.AddHours(9).AddMinutes(15) },
        new Message { Id = new Guid("eeeeeeee-0003-0000-0000-000000000003"), SenderId = testUserId, ReceiverId = otherUserId, MessageText = "I have Pride and Prejudice — interested?", IsRead = false, CreatedAt = now.AddHours(9).AddMinutes(30) },
        new Message { Id = new Guid("eeeeeeee-0004-0000-0000-000000000004"), SenderId = otherUserId, ReceiverId = testUserId, MessageText = "That works great for me. Let's set up the exchange!", IsRead = true, CreatedAt = now.AddHours(9).AddMinutes(45) },
        new Message { Id = new Guid("eeeeeeee-0005-0000-0000-000000000005"), SenderId = testUserId, ReceiverId = otherUserId, MessageText = "Perfect. I'll submit the exchange request now. Check your notifications!", IsRead = false, CreatedAt = now.AddHours(10) }
    );

    await db.SaveChangesAsync();
}

static async Task SeedNotificationsAsync(
    ApplicationDbContext db,
    Guid testUserId,
    Guid otherUserId)
{
    var notif1 = new Guid("ffffffff-0001-0000-0000-000000000001");
    if (await db.Notifications.AnyAsync(n => n.Id == notif1)) return;

    db.Notifications.AddRange(
        new Notification { Id = notif1, UserId = testUserId, Category = NotificationCategory.MatchFound, Title = "Book Match Found", Message = "A book matching your wishlist is now available.", IsRead = false, ReadAt = null, CreatedAt = new DateTime(2025, 1, 9, 8, 0, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0002-0000-0000-000000000002"), UserId = otherUserId, Category = NotificationCategory.NewMessage, Title = "New Message", Message = "You have a new message from test@test.com.", IsRead = true, ReadAt = new DateTime(2025, 1, 10, 9, 16, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0003-0000-0000-000000000003"), UserId = otherUserId, Category = NotificationCategory.ExchangeRequested, Title = "New Exchange Request", Message = "test@test.com has requested an exchange for your listing.", IsRead = false, ReadAt = null, CreatedAt = new DateTime(2025, 1, 10, 10, 5, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0004-0000-0000-000000000004"), UserId = testUserId, Category = NotificationCategory.ExchangeAccepted, Title = "Exchange Accepted", Message = "Your exchange request has been accepted by otheruser@test.com.", IsRead = true, ReadAt = new DateTime(2025, 1, 10, 10, 30, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 10, 10, 20, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0005-0000-0000-000000000005"), UserId = testUserId, Category = NotificationCategory.TransactionUpdate, Title = "Transaction Status Updated", Message = "Your transaction status has changed to Shipped.", IsRead = false, ReadAt = null, CreatedAt = new DateTime(2025, 1, 12, 14, 0, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0006-0000-0000-000000000006"), UserId = otherUserId, Category = NotificationCategory.WishlistAvailable, Title = "Wishlist Book Available", Message = "A book on your wishlist has been listed by another user.", IsRead = false, ReadAt = null, CreatedAt = new DateTime(2025, 1, 13, 9, 0, 0, DateTimeKind.Utc) },
        new Notification { Id = new Guid("ffffffff-0007-0000-0000-000000000007"), UserId = testUserId, Category = NotificationCategory.ExchangeRejected, Title = "Exchange Rejected", Message = "Your exchange request was declined.", IsRead = true, ReadAt = new DateTime(2025, 1, 14, 11, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 14, 10, 30, 0, DateTimeKind.Utc) }
    );

    await db.SaveChangesAsync();
}