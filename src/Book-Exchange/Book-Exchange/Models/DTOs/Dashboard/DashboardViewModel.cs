using Book_Exchange.Models;

namespace Book_Exchange.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int TradeCount { get; set; }

    public int ActiveListingCount { get; set; }
    public int WishlistCount { get; set; }
    public int OpenTransactionCount { get; set; }
    public int UnreadMessageCount { get; set; }

    public List<Transaction> OpenTransactions { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();

    public List<DashboardWishlistItemViewModel> WishlistItems { get; set; } = new();
    public List<DashboardListingItemViewModel> MyActiveListings { get; set; } = new();
}

public class DashboardWishlistItemViewModel
{
    public Guid Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string BookTitle { get; set; } = "Unknown Title";
    public string? BookAuthor { get; set; }
    public bool IsActive { get; set; }
}

public class DashboardListingItemViewModel
{
    public Guid Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string BookTitle { get; set; } = "Unknown Title";
    public string? BookAuthor { get; set; }
    public BookCondition Condition { get; set; }
    public decimal Price { get; set; }
}