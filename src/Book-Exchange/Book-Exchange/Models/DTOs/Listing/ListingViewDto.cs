using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Book;

namespace Book_Exchange.Models.DTOs.Listing;

public class ListingViewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Isbn { get; set; } = null!;
    public BookCondition Condition { get; set; }
    public decimal Price { get; set; }
    public int WeightGrams { get; set; }
    public DateTime CreatedAt { get; set; }

    public BookInfoDto? Book { get; set; }

    public string SellerName { get; set; } = "Unknown";
    public double SellerRating { get; set; } = 0;
    public int SellerReviewerCount { get; set; } = 0;
    public int SellerTradeCount { get; set; } = 0;


}