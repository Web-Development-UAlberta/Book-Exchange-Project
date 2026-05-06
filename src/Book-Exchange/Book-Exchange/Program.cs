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

// Added to troubleshoot GitHub Actions UI test run failures - can be removed once we confirm the fix
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
    var testEmail = "test@test.com";
    if (await userManager.FindByEmailAsync(testEmail) == null)
    {
        var testUser = new ApplicationUser
        {
            UserName = testEmail,
            Email = testEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(testUser, "Test1234!");
    }
}
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();