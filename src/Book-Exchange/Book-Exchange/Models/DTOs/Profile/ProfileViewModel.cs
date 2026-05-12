using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;

namespace Book_Exchange.Models.ViewModels;


public class ProfileViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;

    public DateTime MemberSince { get; set; }

    public string? AvatarUrl { get; set; }

    public double AverageRating { get; set; }

    public int TradeCount { get; set; }

    public int ListingCount { get; set; }

    public List<ProfileReviewItem> Reviews { get; set; } = new();

    public List<ProfileListingItem> Listings { get; set; } = new();

    public bool IsOwnProfile { get; set; }
}

/// <summary>Review row used on the profile page.</summary>
public class ProfileReviewItem
{
    public string ReviewerUserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Listing row used on the profile page.</summary>
public class ProfileListingItem
{
    public Guid Id { get; set; }
    public string Isbn { get; set; } = string.Empty;

    public string BookTitle { get; set; } = "Unknown Title";

    public BookCondition Condition { get; set; }
    public decimal Price { get; set; }
}