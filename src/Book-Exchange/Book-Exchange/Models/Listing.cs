namespace Book_Exchange.Models;

public class Listing
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public BookCondition Condition { get; set; }
    public decimal Price { get; set; }
    public int WeightGrams { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ListingGenre> ListingGenres { get; set; } = new List<ListingGenre>();
    public ICollection<ExchangeRequest> TargetExchangeRequests { get; set; } = new List<ExchangeRequest>();
    public ICollection<ExchangeRequestItem> OfferedInExchangeRequestItems { get; set; } = new List<ExchangeRequestItem>();
}