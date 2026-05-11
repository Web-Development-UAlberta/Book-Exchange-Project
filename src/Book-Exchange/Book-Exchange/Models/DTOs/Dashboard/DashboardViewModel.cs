using Book_Exchange.Models;

namespace Book_Exchange.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int TradeCount { get; set; }

    // Cards
    public int ActiveListingCount { get; set; }
    public int WishlistCount { get; set; }
    public int OpenTransactionCount { get; set; }
    public int UnreadMessageCount { get; set; }

    // Panels
    public List<Transaction> OpenTransactions { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();
    public List<WishlistItem> WishlistItems { get; set; } = new();
    public List<Listing> MyActiveListings { get; set; } = new();
}