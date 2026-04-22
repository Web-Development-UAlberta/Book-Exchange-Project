using Book_Exchange.Data;
using Book_Exchange.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<BookCondition>("public.book_condition");
dataSourceBuilder.MapEnum<ListingType>("public.listing_type");
dataSourceBuilder.MapEnum<TransactionType>("public.transaction_type");
dataSourceBuilder.MapEnum<TransactionStatus>("public.transaction_status");
dataSourceBuilder.MapEnum<ShippingStatus>("public.shipping_status");
dataSourceBuilder.MapEnum<NotificationType>("public.notification_type");
dataSourceBuilder.MapEnum<NotificationStatus>("public.notification_status");
dataSourceBuilder.MapEnum<MessageType>("public.message_type");
dataSourceBuilder.MapEnum<MessageStatus>("public.message_status");
dataSourceBuilder.MapEnum<LocalityType>("public.locality_type");

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();